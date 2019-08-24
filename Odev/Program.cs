using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odev
{
    class Program
    {
        static readonly List<char> columnList = new List<char> { 'P', 'Q', 'R' };
        static readonly List<char> charList = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I' };

        static void Main(string[] args)
        {
            List<CharPosition> startPositions = new List<CharPosition>();
            List<CharPosition> endPositions = new List<CharPosition>();

        Return1:
            Console.WriteLine("Başlangıç durumu için 2-9 arasında blok sayısı seçin.");
            int blockCount = new int();
            if (!int.TryParse(Console.ReadLine(), out blockCount) || blockCount < 2 || blockCount > 9)
                goto Return1;

            SetStartPosition(startPositions, blockCount);
            SetEndPosition(endPositions, blockCount);

            Console.WriteLine("");
            Console.WriteLine("İlk durum");
            foreach (var item in startPositions.OrderBy(x => x.Character))
                Console.WriteLine(item.ToString());
            Console.WriteLine("");

            List<CharPosition> result = new List<CharPosition>();
            while (startPositions.Count > 0)
            {
                foreach (var column in columnList)
                {
                    foreach (var endPosition in endPositions.Where(x => x.Column == (Column)column).OrderBy(x => x.Index))
                    {
                        bool search = true;
                        while (search)
                        {
                            var startPosition = startPositions.FirstOrDefault(x => x.Column == endPosition.Column && x.Character == endPosition.Character && x.Index == endPosition.Index);
                            if (startPosition != null)
                            {
                                Console.WriteLine(startPosition.Character + " bitiş pozisyonunda");
                                result.Add(startPosition);
                                startPositions.Remove(startPosition);
                                search = false;
                            }
                            else
                            {
                                CharPosition @char = startPositions.FirstOrDefault(x => x.Character == endPosition.Character);
                                ClearTargetColumn(endPosition, @char, startPositions);
                                ClearCharColumn(endPosition.Column, @char, startPositions);
                                MoveToColumn(@char, endPosition.Column);
                                result.Add(@char);
                                startPositions.Remove(@char);
                                search = false;
                            }
                        }
                    }
                }
            }

            Console.WriteLine("");
            Console.WriteLine("İstenen");
            foreach (var item in endPositions.OrderBy(x => x.Character))
                Console.WriteLine(item.ToString());

            Console.WriteLine("--------------------------");
            Console.WriteLine("--------------------------");
            Console.WriteLine("Sonuç");
            foreach (var item in result.OrderBy(x => x.Character))
                Console.WriteLine(item.ToString());
            Console.ReadKey();
        }

        static void ClearCharColumn(Column column, CharPosition @char, List<CharPosition> startPositions)
        {
            foreach (var item in startPositions.
                Where(x => x.Index > @char.Index))
            {
                MoveToColumn(item, ((IEnumerable<Column>)Enum.GetValues(typeof(Column))).FirstOrDefault(x => x != column && x != @char.Column));
            }
        }

        static void ClearTargetColumn(CharPosition target, CharPosition @char, List<CharPosition> startPositions)
        {
            if (target.Column == @char.Column && @char.Index > target.Index)//hedef kolonda işlem bloğu olması gerekenden yukarıdaysa
            {
                Column column = new Column();
                foreach (var item in startPositions//hedef kolondaki işlem bloğu üzerindeki blokların taşınması
                    .Where(x => x.Column == @char.Column &&
                    x.Index > @char.Index)
                    .OrderByDescending(x => x.Index))
                {
                    column = MoveNext(item);
                }

                MoveToColumn(@char,//işlem bloğu diğer kolona atlatılır
                    ((IEnumerable<Column>)Enum.GetValues(typeof(Column))).FirstOrDefault(x => x != target.Column && x != column));

                foreach (var item in startPositions//hedef kolondaki işlem bloğu altındaki blokların taşınması
                    .Where(x => x.Column == @char.Column)
                    .OrderByDescending(x => x.Index))
                {
                    MoveNext(item);
                }
            }
            foreach (var item in startPositions.
                Where(x => x.Column == target.Column &&
                x.Character != @char.Character))
            {
                MoveToColumn(item, ((IEnumerable<Column>)Enum.GetValues(typeof(Column))).FirstOrDefault(x => x != target.Column && x != @char.Column));
            }
        }

        static Column MoveNext(CharPosition item)
        {
            char nextColumnChar = (char)item.Column;
            nextColumnChar = (char)(nextColumnChar + 1);
            Column nextColumn = columnList.Contains(nextColumnChar) ? (Column)nextColumnChar : item.Column - 1;
            Console.WriteLine($"{item.Character} bloğu {(char)item.Column} kolonundan {(char)nextColumn} kolonuna hareket etti");
            item.Column = nextColumn;
            return nextColumn;
        }

        static void MoveToColumn(CharPosition @char, Column column)
        {
            Console.WriteLine($"{@char.Character} bloğu {(char)@char.Column} kolonundan {(char)column} kolonuna hareket etti");
            @char.Column = column;
        }

        static void SetStartPosition(List<CharPosition> startPositions, int blockCount)
        {
            Console.WriteLine("Başlangıç pozisyonu;");
            List<char> newList = charList.Take(blockCount).ToList();//blok sayısına göre char listesinden karakterler alınır
            for (int i = 0; i < blockCount; i++)
            {
            Return2:
                char _char = new char();
                if (newList.Count != 1)
                {
                    Console.WriteLine(string.Join(", ", newList.ToArray()) + " karakterlerinden birini seçin.");
                    if (!char.TryParse(Console.ReadLine().ToUpper(), out _char) || !newList.Contains(_char))
                        goto Return2;
                }

            Return3:
                if (newList.Count == 1)
                    Console.WriteLine(newList.First().ToString() + " karakteri için başlangıç kolonunu seçin. Sırayla => " + string.Join(", ", columnList));
                else
                    Console.WriteLine(_char.ToString().ToUpper() + " karakterinin başlangıç kolonunu seçin. Sırayla => " + string.Join(", ", columnList));
                char column = new char();
                if (!char.TryParse(Console.ReadLine().ToUpper(), out column) || blockCount < 2 || blockCount > 9)
                    goto Return3;

                startPositions.Add(new CharPosition
                {
                    Character = newList.Count == 1 ? newList.First() : _char,
                    Column = (Column)column,
                    Index = startPositions.Count(x => x.Column == (Column)column) + 1
                });
                newList.Remove(_char);
            }
        }

        static void SetEndPosition(List<CharPosition> endPositions, int blockCount)
        {
            Console.WriteLine("Bitiş pozisyonu;");
            List<char> newList = charList.Take(blockCount).ToList();//blok sayısına göre char listesinden karakterler alınır
            for (int i = 0; i < blockCount; i++)
            {
            Return2:
                char _char = new char();
                if (newList.Count != 1)
                {
                    Console.WriteLine(string.Join(", ", newList.ToArray()) + " karakterlerinden birini seçin.");
                    if (!char.TryParse(Console.ReadLine().ToUpper(), out _char) || !newList.Contains(_char))
                        goto Return2;
                }

            Return3:
                if (newList.Count == 1)
                    Console.WriteLine(newList.First().ToString() + " karakteri için başlangıç kolonunu seçin. Sırayla => " + string.Join(", ", columnList));
                else
                    Console.WriteLine(_char.ToString().ToUpper() + " karakterinin başlangıç kolonunu seçin. Sırayla => " + string.Join(", ", columnList));
                char column = new char();
                if (!char.TryParse(Console.ReadLine().ToUpper(), out column) || blockCount < 2 || blockCount > 9)
                    goto Return3;

                endPositions.Add(new CharPosition
                {
                    Character = newList.Count == 1 ? newList.First() : _char,
                    Column = (Column)column,
                    Index = endPositions.Count(x => x.Column == (Column)column) + 1
                });
                newList.Remove(_char);
            }
        }
    }

    class CharPosition
    {
        public char Character { get; set; }
        public Column Column { get; set; }
        public int Index { get; set; }

        public override string ToString()
        {
            return "Blok:" + Character + " Kolon:" + (char)Column + " Sıra:" + Index;
        }
    }

    enum Column
    {
        First = 'P',
        Second = 'Q',
        Third = 'R'
    }
}
