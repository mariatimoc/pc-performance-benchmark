using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Text;
using System.Runtime.InteropServices;



namespace PerformanceGUI
{
    public struct CACHE_MEMORY
    {
        public int type;
        public int level;
        public int selfInitializing;
        public int fullyAssociative;
        public int numberThreads;
        public int numberCores;

        public int sizeLine;
        public int numberLinePartitions;
        public int numberAsociativityPaths;

        public int totalNumberOfSets;

        public int isInclusiveCache;
        public int complexIndexing;
    }


   
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SYSINFO
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string architecture;
        public uint logicalProcessors;
        public ulong totalRamMb;
    }


    public partial class MainWindow : Window
        {

            [DllImport("Proiect_SSC.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void cpu_get_producer_name([Out] StringBuilder producerName);

            [DllImport("Proiect_SSC.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            private static extern void cpu_get_brand([Out] StringBuilder brand);

            [DllImport("Proiect_SSC.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern void cpu_get_basics(out int family, out int model, out int stepping, out int type);

            [DllImport("Proiect_SSC.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern void cpu_get_address_size(out int physBits, out int virtBits);

            [DllImport("Proiect_SSC.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern int cpu_get_frequencies(out int baseMHz, out int maxMHz, out int busMHz);

            [DllImport("Proiect_SSC.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern int cpu_get_cache_all_levels([Out] CACHE_MEMORY[] caches, int maxCaches);

            [DllImport("Proiect_SSC.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern double test_memory_random_access(int n, int stepsPerElement);

            [DllImport("Proiect_SSC.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern double test_gauss_int_performance(int n);

            [DllImport("Proiect_SSC.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern double test_floating_numbers_performance(int width, int height, int iterations);

            [DllImport("Proiect_SSC.dll", CallingConvention = CallingConvention.Cdecl)]
            private static extern void sys_get_information(out SYSINFO info);


            private List<TestDefinition> tests = new List<TestDefinition>()
            {
                new TestDefinition(0, "CPU info"),
                new TestDefinition(1, "System info"),
                new TestDefinition(2, "Memory random access"),
                new TestDefinition(3, "Gauss int"),
                new TestDefinition(4, "Floating numbers")
            };


            private TestDefinition selected;
            private TextBlock selectedTest;
            private TextBox numberRepsBox;
            ListBox testsList = new ListBox();


            private void BuildUI()
            {
                //containee principal
                Grid grid = new Grid { Margin = new Thickness(12) }; //creez obiectul si setez prop cu {}
                //margin =distanta fata de margine
                //padding=distanta intre bordura si continut

                //impart fereastra in 3 coloane,stabilesc dimensiunea fiecareia
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(260) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(12) });
                grid.ColumnDefinitions.Add(new ColumnDefinition
                    { Width = new GridLength(1, GridUnitType.Star) }); //restul de spatiu ramas pt coloana 2

                //=========partea din stanga
                //cutia cu butoane
                GroupBox leftGroup = new GroupBox { Header = "Teste" };
                Grid.SetColumn(leftGroup, 0); //cutia vine pusa in partea stanga

                ScrollViewer scroll = new ScrollViewer
                    { VerticalScrollBarVisibility = ScrollBarVisibility.Auto }; //creez container cu scroll
                StackPanel leftStack = new StackPanel(); //container care pune elem unul sub altul,aici ad butoanele

                scroll.Content = leftStack; //continutul scrolului e leftstack
                leftGroup.Content = scroll; //continut leftgroup e scroll


                testsList.Margin = new Thickness(4);


                for (int i = 0; i < tests.Count; i++)
                {
                    testsList.Items.Add(tests[i]);
                }

                testsList.SelectionChanged += testSelected;
                leftStack.Children.Add(testsList);


                //===================partea din dreapta
                GroupBox rightGroup = new GroupBox { Header = "Rulare" };
                Grid.SetColumn(rightGroup, 2);

                StackPanel rightStack = new StackPanel
                {
                    Margin = new Thickness(10) //lasa spatiu de 10 pixeli pe toate partile
                };
                rightGroup.Content = rightStack;


                rightStack.Children.Add(new TextBlock { Text = "Test selectat: " }); //textblock e ca un label,nume fix

                //textul  care se schimba
                selectedTest = new TextBlock
                {
                    FontWeight = FontWeights.SemiBold,
                    Margin = new Thickness(0, 0, 0, 10)
                };
                rightStack.Children.Add(selectedTest); //pun efectiv in interfata


                StackPanel repsRow = new StackPanel
                {
                    Orientation = Orientation.Horizontal, //aranjare pe orizontala
                    Margin = new Thickness(0, 0, 0, 10)
                };

                repsRow.Children.Add(new TextBlock
                {
                    Text = "Repetitii:",
                    VerticalAlignment = VerticalAlignment.Center
                });


                numberRepsBox = new TextBox { Width = 120, Margin = new Thickness(10, 0, 0, 0), Text = "100" };
                repsRow.Children.Add(numberRepsBox);
                rightStack.Children.Add(repsRow);

                Button runOnceButton = new Button
                {
                    Content = "Run once",
                    Padding = new Thickness(12, 6, 12, 6),
                    Margin = new Thickness(0, 0, 0, 8)
                };

                runOnceButton.Click += runOnceClick;
                rightStack.Children.Add(runOnceButton);


                Button averageButton = new Button
                {
                    Content = "Run average",
                    Padding = new Thickness(12, 6, 12, 6),

                };

                averageButton.Click += runAverageClick;
                rightStack.Children.Add(averageButton);


                grid.Children.Add(leftGroup);
                grid.Children.Add(rightGroup);

                Content = grid;
                if (tests.Count > 0)
                {
                    testsList.SelectedIndex = 0;
                    selected = testsList.SelectedItem as TestDefinition;
                    selectedTest.Text = selected?.Name ?? "";
                }


            }

            private int GetRuns()
            {
                int runs;
                if (!int.TryParse(numberRepsBox.Text, out runs) || runs <= 0)
                    runs = 10;
                return runs;
            }


         private void runOnceClick(object sender, RoutedEventArgs e)
           {
               if (selected == null)
               {
                   MessageBox.Show("Pick a test");
                   return;
               }


               if (selected.Id == 0)
               {
                   StringBuilder producerName = new StringBuilder(13);
                   StringBuilder brand = new StringBuilder(49);

                   cpu_get_producer_name(producerName);
                   cpu_get_brand(brand);

                   cpu_get_basics(out int family, out int model, out int stepping, out int type);


                   cpu_get_address_size(out int physBits, out int virtBits);


                   int hasFreq = cpu_get_frequencies(out int baseMHz, out int maxMHz, out int busMHz);


                   CACHE_MEMORY[] caches = new CACHE_MEMORY[5];
                   int cacheCount = cpu_get_cache_all_levels(caches, caches.Length);

                   StringBuilder sb = new StringBuilder();
                   sb.AppendLine("Producer: " + producerName);
                   sb.AppendLine("Brand: " + brand);
                   sb.AppendLine("Family: " + family);
                   sb.AppendLine("Model: " + model);
                   sb.AppendLine("Stepping: " + stepping);
                   sb.AppendLine("Type: " + type);
                   sb.AppendLine();

                   sb.AppendLine($"Address size: physical={physBits} bits, virtual={virtBits} bits");

                   if (hasFreq == 1)
                       sb.AppendLine($"Frequencies: base={baseMHz} MHz, max={maxMHz} MHz, bus={busMHz} MHz");

                   sb.AppendLine();
                   sb.AppendLine("Cache all levels: " + cacheCount);

                   for (int i = 0; i < cacheCount; i++)
                   {
                       CACHE_MEMORY c = caches[i];
                       long sizeBytes = (long)c.sizeLine * c.numberLinePartitions * c.numberAsociativityPaths *
                                        c.totalNumberOfSets;

                       sb.AppendLine(
                           $"- L{c.level} type={c.type} size={sizeBytes / 1024} KB " +
                           $"line={c.sizeLine} ways={c.numberAsociativityPaths} sets={c.totalNumberOfSets} " +
                           $"threads={c.numberThreads} cores={c.numberCores} " +
                           $"inclusive={c.isInclusiveCache} complexIndex={c.complexIndexing}"
                       );
                   }

                   MessageBox.Show(sb.ToString(), "CPU basics (all info)");
                   return;
               }


               if (selected.Id == 1)
               {
                   sys_get_information(out SYSINFO info);

                   StringBuilder sb = new StringBuilder();
                   sb.AppendLine("Architecture: " + info.architecture);
                   sb.AppendLine("Logical processors: " + info.logicalProcessors);
                   sb.AppendLine("Total RAM: " + info.totalRamMb + " MB");

                   MessageBox.Show(sb.ToString(), "System info");
                   return;
               }

               if (selected.Id == 2)
               {
                   int stepsPerElement = GetRuns();  
                   int n = 1000000;                 

                   double ms = test_memory_random_access(n, stepsPerElement);

                   MessageBox.Show(
                       "Memory random access (Run once)\n" +
                       "n = " + n + "\n" +
                       "stepsPerElement = " + stepsPerElement + "\n" +
                       "Time = " + ms.ToString("0.00") + " ms",
                       "Memory random access"
                   );
                   return;
               }

               if (selected.Id == 3)
               {
                   int n = GetRuns();
                   if (n < 2)
                   {
                       MessageBox.Show("Trebuie sa introduceti dimensiune> 2");
                       return;
                   }

                   double ms = test_gauss_int_performance(n);
                   MessageBox.Show("Gauss int (Run once)\n" +
                                   "n= " + n + "\n" +
                                   "Time= " + ms.ToString("0.00") + "ms",
                       "Gauss int");
                   return;
               }

               if (selected.Id == 4)
               {
                   int iterations = GetRuns();
                   int width = 1024, height = 1024;

                   double ms = test_floating_numbers_performance(width, height, iterations);

                   MessageBox.Show(
                       "Floating numbers (Run once)\n" +
                       $"width = {width}\nheight = {height}\niterations = {iterations}\n" +
                       $"Time = {ms:0.00} ms",
                       "Floating numbers"
                   );
                   return;
               }

           }
        

        private void runAverageClick(object sender, RoutedEventArgs e)
            {
                try
                    {
                    if(selected == null)
                    {
                        MessageBox.Show("Pick a test");
                        return;
                    }

                        int n = GetRuns();

                    if(selected.Id == 0 || selected.Id == 1)
                    {
                        MessageBox.Show($"Testul \"{selected.Name}\" nu e test de performanta deci nu are average", "Info");
                        return;
                    }


                    if (selected.Id == 2)
                    {
                        int dim= 1000000;
                        int stepsPerElement = 100;

                        test_memory_random_access(dim, stepsPerElement);

                        double sum = 0;
                        double min = double.MaxValue;
                        double max = double.MinValue;

                        for (int i = 0; i < n; i++)
                        {
                            double ms = test_memory_random_access(n, stepsPerElement);

                            sum += ms;
                            if (ms < min) 
                                min = ms;
                            if (ms > max) 
                                max = ms;
                        }

                        double avg = sum / n;
                        MessageBox.Show(
                            "Memory random access (Average)\n" +
                                "runs = " + n + "\n" +
                                "n = " + n + "\n" +
                             "stepsPerElement = " + stepsPerElement + "\n\n" +
                            "avg = " + avg.ToString("0.00") + " ms\n" +
                            "min = " + min.ToString("0.00") + " ms\n" +
                            "max = " + max.ToString("0.00") + " ms",
                            "Memory random access"
                        );
                        return;
                    }

                    if (selected.Id == 3)
                    {
                        
                        if (n < 2)
                        {
                            MessageBox.Show("Trebuie sa introduceti dimensiune>2");
                            return;
                        }
                        double sum = 0;
                        double min = double.MaxValue;
                        double max = double.MinValue;
                        for(int i=0;i<n;i++)
                        {
                            double ms = test_gauss_int_performance(n);
                            sum += ms;
                            if (ms < min) 
                                min = ms;
                            if (ms > max)
                                max = ms;
                        }

                        double avg = sum / n;

                        MessageBox.Show("Gauss int (Run once)\n" +
                                        "n= " + n + "\n" +
                                        "avg = " + avg.ToString("0.00") + " ms\n" +
                                        "min = " + min.ToString("0.00") + " ms\n" +
                                        "max = " + max.ToString("0.00") + " ms",
                            "Gauss int");
                        return;
                    }


                    if (selected.Id == 4)
                    {
                        int width = 1024, height = 1024, iterations = 50;

                        double sum = 0;
                        double min = double.MaxValue;
                        double max = double.MinValue;

                        for (int i = 0; i < n; i++)
                        {
                            double ms = test_floating_numbers_performance(width, height, iterations);
                            sum += ms;
                            if (ms < min) min = ms;
                            if (ms > max) max = ms;
                        }

                        double avg = sum / n;

                        MessageBox.Show(
                            "Floating numbers (Average)\n" +
                            $"runs = {n}\n" +
                            $"width = {width}\nheight = {height}\niterations = {iterations}\n\n" +
                            $"avg = {avg:0.00} ms\n" +
                            $"min = {min:0.00} ms\n" +
                            $"max = {max:0.00} ms",
                            "Floating numbers"
                        );
                        return;
                    }

                }
                catch (Exception ex)

                {
                    MessageBox.Show(ex.ToString(), "Run once error");
                }
            }

            private void testSelected(object sender, SelectionChangedEventArgs e)
            {
                TestDefinition test = testsList.SelectedItem as TestDefinition;
                if (test != null)
                {
                    selected = test;
                    if (selectedTest != null) 
                        selectedTest.Text = test.Name;
                }
            }

            public MainWindow()
            {
                InitializeComponent();
                BuildUI();
            }

        }

        public class TestDefinition
        {
            public int Id { get; private set; }
            public string Name { get; private set; }

            public TestDefinition(int id, string name)
            {
                Id = id;
                Name = name;
            }

            public override string ToString()
            {
                return Name;
            }
        }
}