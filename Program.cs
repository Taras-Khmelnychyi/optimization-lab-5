using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Lab2
{
    class Program
    {
        static void Main()
        {
            //MyMatrix A = new MyMatrix(2);
            //MyMatrix B = new MyMatrix(2);
            //A.HandInit("A");
            //B.HandInit("B");
            //(A * B).ShowMatrix("A + B"); 
            MainProcess prc = new MainProcess();
            prc.Start();
        }
    }

    class MainProcess
    {
        int n;
        public MyMatrix A1, A2, B2, C2, A, b1, c1, bi, Y3, y1, y2, Y3squared, Y3cubed;
        public MyMatrix comp2_1, comp2_2, comp4_1, comp4_2, comp5_2, comp5_3, comp6_1, comp6_2;
        public MyMatrix result;

        public void Start()
        {

            Console.WriteLine("Enter n: ");
            n = Convert.ToInt32(Console.ReadLine());
            InitStage();
        }

        private void InitStage()
        {
            //init stage
            //initialised by formula
            bi = new MyMatrix(n);
            var create_bi = Task.Factory.StartNew(() => { bi.Create_bi(); });
            C2 = new MyMatrix(n);
            var create_C2 = Task.Factory.StartNew(() => { C2.Create_C2(); });

            Console.WriteLine("Initialise random? y/n");
            ConsoleKey answer = Console.ReadKey(true).Key;

            if (answer == ConsoleKey.Y)
            {
                A = new MyMatrix(n);
                A1 = new MyMatrix(n);
                A2 = new MyMatrix(n);
                B2 = new MyMatrix(n);
                b1 = new MyMatrix(n);
                c1 = new MyMatrix(n);
                //creating and starting new task for each initial matrix to run in sync
                var create_A = Task.Factory.StartNew(() => { A.RandomInitMatrix(); });
                var create_A1 = Task.Factory.StartNew(() => { A1.RandomInitMatrix(); });
                var create_A2 = Task.Factory.StartNew(() => { A2.RandomInitMatrix(); });
                var create_B2 = Task.Factory.StartNew(() => { B2.RandomInitMatrix(); });
                var create_b1 = Task.Factory.StartNew(() => { b1.RandomInitVector(); });
                var create_c1 = Task.Factory.StartNew(() => { c1.RandomInitVector(); });
                Task.WaitAll();
            }
            else
            {
                Console.WriteLine("Write row elements in a single line separeted by space");
                A = new MyMatrix(n);
                A1 = new MyMatrix(n);
                A2 = new MyMatrix(n);
                B2 = new MyMatrix(n);
                b1 = new MyMatrix(n);
                c1 = new MyMatrix(n);
                A.HandInit("A");
                A1.HandInit("A1");
                A2.HandInit("A2");
                B2.HandInit("B2");
                b1.HandInit("b1");
                c1.HandInit("c1");
            }
            MainSync();
        }

        private void Calc_comp2_1()
        {
            comp2_1 = ((b1 * 3) + c1);
            
        }

        private void Calc_comp2_2()
        {
            comp2_2 = B2 + C2;
        }

        private void Calc_y1()
        {
            y1 = A * bi;
        }

        private void Calc_y2() 
        {
            y2 = A1 * comp2_1;
        }

        private void Calc_Y3()
        {
            Y3 = A2 * comp2_2;
        }

        private void Calc_Y3squared()
        {
            Y3squared = Y3 * Y3;
        }

        private void Calc_comp4_1()
        {
            comp4_1 = y2 * y2.GetTransposed();
        }

        private void Calc_comp4_2()
        {
            comp4_2 = y2 * y1.GetTransposed();
        }

        private void Calc_Y3cubed()
        {
            Y3cubed = Y3squared * Y3;
        }

        private void Calc_comp5_2()
        {
            comp5_2 = Y3 * comp4_1;
        }

        private void Calc_comp5_3()
        {
            comp5_3 = Y3squared * y1.GetTransposed();
        }

        private void Calc_comp6_1()
        {
            comp6_1 = comp5_2 + Y3cubed;
        }

        private void Calc_comp6_2()
        {
            comp6_2 = Y3 + comp4_2 + comp5_3;
        }

        private void Calc_result()
        {
            result = comp6_1 - comp6_2;
        }

        //A1, A2, B2, C2, A, b1, c1, bi, Y3, y1, y2, Y3squared, Y3cubed;
        //comp2_1, comp2_2, comp4_1, comp4_2, comp5_2, comp5_3, comp6_1, comp6_2;
        private void MainSync()
        {
            Thread t_comp2_1 = new Thread(Calc_comp2_1);
            t_comp2_1.Start();
            Thread t_comp2_2 = new Thread(Calc_comp2_2);
            t_comp2_2.Start();
            Thread t_y1 = new Thread(Calc_y1);
            t_y1.Start();

            //stage 3 processes y2, Y3
            t_comp2_1.Join();
            Thread t_y2 = new Thread(Calc_y2);
            t_y2.Start();
            t_comp2_2.Join();
            Thread t_Y3 = new Thread(Calc_Y3);
            t_Y3.Start();

            //stage 4 process Y3^2, (y2 * y2'), (y2 * y1')
            t_Y3.Join();
            Thread t_Y3squared = new Thread(Calc_Y3squared);
            t_Y3squared.Start();
            t_y2.Join();
            Thread t_comp4_1 = new Thread(Calc_comp4_1);
            t_comp4_1.Start();
            t_y1.Join();
            Thread t_comp4_2 = new Thread(Calc_comp4_2);
            t_comp4_2.Start();

            //stage 5 process (Y3^3),  (Y3 * y2 * y2'), (Y3^2 * y1')
            t_Y3squared.Join();
            Thread t_Y3cubed = new Thread(Calc_Y3cubed);
            t_Y3cubed.Start();
            t_comp4_1.Join();
            Thread t_comp5_2 = new Thread(Calc_comp5_2);
            t_comp5_2.Start();
            Thread t_comp5_3 = new Thread(Calc_comp5_3);
            t_comp5_3.Start();



            t_comp5_2.Join();
            t_Y3cubed.Join();
            Thread t_comp6_1 = new Thread(Calc_comp6_1);
            t_comp6_1.Start();
            t_comp4_2.Join();
            t_comp5_3.Join();
            Thread t_comp6_2 = new Thread(Calc_comp6_2);
            t_comp6_2.Start();


            t_comp6_1.Join();
            t_comp6_2.Join();
            Thread t_result = new Thread(Calc_result);
            t_result.Start();
            t_result.Join();
            ////stage 6 process 
            //List<Task> tasksTo_comp6_1 = new List<Task> { comp5_2, Y3_cubed };
            //var comp6_1 = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_comp6_1.ToArray(), (some) =>
            //{
            //    return comp5_2.Result + Y3_cubed.Result;
            //});
            //List<Task> tasksTo_comp6_2 = new List<Task> { Y3, comp4_2, comp5_3 };
            //var comp6_2 = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_comp6_2.ToArray(), (some) =>
            //{
            //    return Y3.Result + comp4_2.Result + comp5_3.Result;
            //});

            ////Last stage
            //List<Task> lastTask = new List<Task> { comp6_1, comp6_2 };
            //var result = Task<MyMatrix>.Factory.ContinueWhenAll(tasksTo_comp6_2.ToArray(), (some) =>
            //{
            //    return comp6_1.Result - comp6_2.Result;
            //});

            //Task tLast = Task.Factory.ContinueWhenAll(lastTask.ToArray(), (some) =>
            //{
            //    this.result = result.Result;
            //});

            //Task.WaitAll();
            ResultStage();
        }

        private void ResultStage()
        {
            Console.WriteLine("Show matrices? y/n");
            ConsoleKey answer = Console.ReadKey(true).Key;

            //A1, A2, B2, C2, A, b1, c1, bi, Y3, y1, y2, Y3squared, Y3cubed
            if (answer == ConsoleKey.Y)
            {
                Console.WriteLine("Matrices:");
                A1.ShowMatrix("A1");
                A2.ShowMatrix("A2");
                B2.ShowMatrix("B2");
                C2.ShowMatrix("C2");
                A.ShowMatrix("A");
                b1.ShowMatrix("b1");
                c1.ShowMatrix("c1");
                bi.ShowMatrix("bi");
                Y3.ShowMatrix("Y3");
                y1.ShowMatrix("y1");
                y2.ShowMatrix("y2");
                result.ShowMatrix("result");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
