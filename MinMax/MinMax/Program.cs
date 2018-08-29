using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace MinMax {

    public class MinMax {
        Random rnd = new Random(31337);

        int[] GenerateRandom(int length) {
            int[] result = new int[length];
            for (int i = 0; i < length; i++) {
                result[i] = rnd.Next();
            }
            return result;
        }

        int[] GenerateAscending(int length) {
            int[] result = new int[length];
            for (int i = 0; i < length; i++) {
                result[i] = i;
            }
            return result;
        }

        int[] GenerateDescending(int length) {
            int[] result = new int[length];
            for (int i = 0; i < length; i++) {
                result[i] = length - 1 - i;
            }
            return result;
        }

        public int[] ValuesForItemsCount => new[] {4, 5, 100, 101, 10000, 10001};
        public string[] ValuesForItemsOrder => new[] {"Random", "Ascending", "Descending"};

        [ParamsSource(nameof(ValuesForItemsCount))]
        public int ItemsCount { get; set; }

        [ParamsSource(nameof(ValuesForItemsOrder))]
        public string ItemsOrder { get; set; }

        public int[] Data { get; set; }

        [GlobalSetup]
        public void GlobalSetup() {
            if (ItemsOrder == "Random") {
                Data = GenerateRandom(ItemsCount);
            }
            else if (ItemsOrder == "Ascending") {
                Data = GenerateAscending(ItemsCount);
            }
            else if (ItemsOrder == "Descending") {
                Data = GenerateDescending(ItemsCount);
            }
        }

        [Benchmark]
        public Tuple<int, int> SimultaneousMinMax() {
            int[] data = Data;
            int min = data[0];
            int max = data[0];
            int s = data.Length % 2;
            for (int i = s; i < data.Length; i += 2) {
                int a = data[i];
                int b = data[i + 1];
                if (a < b) {
                    if (a < min) {
                        min = a;
                    }
                    if (b > max) {
                        max = b;
                    }
                }
                else {
                    if (b < min) {
                        min = b;
                    }
                    if (a > max) {
                        max = a;
                    }
                }
            }
            return new Tuple<int, int>(min, max);
        }

        [Benchmark]
        public Tuple<int, int> SimultaneousMinMax2() {
            int[] data = Data;
            int min = data[0];
            int max = data[0];
            int s = data.Length % 2;
            for (int i = s; i < data.Length; i += 2) {
                int a = data[i];
                int b = data[i + 1];
                if (a > b) {
                    int t = a;
                    a = b;
                    b = t;
                }
                if (a < min) {
                    min = a;
                }
                if (b > max) {
                    max = b;
                }
            }
            return new Tuple<int, int>(min, max);
        }

        [Benchmark(Baseline = true)]
        public Tuple<int, int> SequentialMinMax() {
            int[] data = Data;
            int min = data[0];
            for (int index = 0; index < data.Length; index++) {
                int i = data[index];
                if (i < min) {
                    min = i;
                }
            }
            int max = data[0];
            for (int index = 0; index < data.Length; index++) {
                int i = data[index];
                if (i > max) {
                    max = i;
                }
            }
            return new Tuple<int, int>(min, max);
        }

        [Benchmark]
        public Tuple<int, int> SequentialMinMaxOneFor() {
            int[] data = Data;
            int min = data[0];
            int max = data[0];
            for (int index = 0; index < data.Length; index++) {
                int i = data[index];
                if (i < min) {
                    min = i;
                }
                if (i > max) {
                    max = i;
                }
            }
            return new Tuple<int, int>(min, max);
        }

        [Benchmark]
        public Tuple<int, int> LinqMinMax() {
            int[] data = Data;
            return new Tuple<int, int>(data.Min(), data.Max());
        }

        public void SelfTest() {
            foreach (int itemsCount in ValuesForItemsCount) {
                ItemsCount = itemsCount;
                foreach (string itemsOrder in ValuesForItemsOrder) {
                    ItemsOrder = itemsOrder;
                    GlobalSetup();
                    var a1 = SimultaneousMinMax();
                    var a2 = SequentialMinMax();
                    var a3 = SequentialMinMaxOneFor();
                    var a4 = LinqMinMax();
                    var a5 = SimultaneousMinMax2();
                    if (a1.Item1 != a2.Item1 || a1.Item2 != a2.Item2) {
                        throw new Exception();
                    }
                    if (a1.Item1 != a3.Item1 || a1.Item2 != a3.Item2) {
                        throw new Exception();
                    }
                    if (a1.Item1 != a4.Item1 || a1.Item2 != a4.Item2) {
                        throw new Exception();
                    }
                    if(a1.Item1 != a5.Item1 || a1.Item2 != a5.Item2) {
                        throw new Exception();
                    }
                }
            }
        }
    }

    class Program {
        static void Main() {
            //MinMax mm = new MinMax();
            //mm.SelfTest();

            BenchmarkRunner.Run<MinMax>();
        }
    }
}
