using System.Collections.Generic;
using System.IO;
using System.Text;
using Aika_BinToJson.Models;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class SetItem : BaseConvert
    {
        public override void Convert()
        {
            using (var stream = new BinaryReader(File.OpenRead(Path), Encode))
            {
                var size = stream.BaseStream.Length;
                ushort i = 0;

                var list = new List<SetItemJson>();
                var txt = new StringBuilder();
                while (stream.BaseStream.Position < size - 4)
                {
                    var temp = new SetItemJson
                    {
                        LoopId = i,
                        Name = Encode.GetString(stream.ReadBytes(64)).Trim('\u0000'),
                        Name2 = Encode.GetString(stream.ReadBytes(64)).Trim('\u0000'),
                        Quantity = stream.ReadUInt16(),
                        Unk1 = stream.ReadUInt16(),
                        ItemsId = new ushort[12],
                        EffectNumber = new ushort[6],
                        Unk = new ushort[6],
                        CastChance = new ushort[6],
                        EffectId = new ushort[6],
                        EffectValue = new ushort[6],
                    };

                    for (var j = 0; j < 12; j++)
                        temp.ItemsId[j] = stream.ReadUInt16();

                    for (var j = 0; j < 6; j++)
                        temp.EffectNumber[j] = stream.ReadUInt16();

                    for (var j = 0; j < 6; j++)
                        temp.Unk[j] = stream.ReadUInt16();

                    for (var j = 0; j < 6; j++)
                        temp.CastChance[j] = stream.ReadUInt16();

                    for (var j = 0; j < 6; j++)
                        temp.EffectId[j] = stream.ReadUInt16();

                    for (var j = 0; j < 6; j++)
                        temp.EffectValue[j] = stream.ReadUInt16();

                    i++;
                    if (string.IsNullOrEmpty(temp.Name)) continue;

                    list.Add(temp);
                    var newName = temp.Name.Replace("'", "''").Trim();
                    var newName2 = temp.Name2.Replace("'", "''").Trim();
                    txt.AppendLine($"INSERT INTO `data_sets` VALUES ({temp.LoopId}, '{newName}', '{newName2}', {temp.Unk1});");
                    for (var j = 0; j < temp.Quantity; j++)
                    {
                        if (temp.ItemsId[j] <= 0) continue;
                        txt.AppendLine($"INSERT INTO `data_set_items` VALUES ({temp.LoopId}, {temp.ItemsId[j]});");
                    }

                    for (var j = 0; j < 6; j++)
                    {
                        if (temp.EffectNumber[j] <= 0) continue;
                        txt.AppendLine($"INSERT INTO `data_set_effects` VALUES ({temp.LoopId}, " +
                                       $"{temp.EffectNumber[j]}, {temp.Unk[j]}, {temp.CastChance[j]}, {temp.EffectId[j]}, {temp.EffectValue[j]});");
                    }
                }

                SqlData = txt.ToString();
                JsonData = JsonConvert.SerializeObject(list, Formatting.Indented);
            }
        }
    }
}