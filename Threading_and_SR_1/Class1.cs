using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Speech.Recognition;

namespace Threading_and_SR_1
{
    class Class1
    {
        //RecognizerInfo info;
        static SpeechRecognitionEngine sre;

        static void Main()
        {
            RecognizerInfo info;
            info = null;
            //sre = null;

            foreach (RecognizerInfo ri in SpeechRecognitionEngine.InstalledRecognizers())
            {
                Console.WriteLine(ri.Culture.Name);
                if (ri.Culture.TwoLetterISOLanguageName.Equals("it"))
                {
                    info = ri;
                    break;
                }
            }
            if (info == null)
            {
                Console.WriteLine("No 'it' recognizer found");
                return;
            }

            sre = new SpeechRecognitionEngine(info);
            sre.SetInputToDefaultAudioDevice();

            sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(handler_SpeechRecognized);
            sre.RecognizeCompleted += new EventHandler<RecognizeCompletedEventArgs>(handler_RecognizeCompleted);

            GrammarBuilder gb = new GrammarBuilder();
            gb.Culture = sre.RecognizerInfo.Culture;

            Choices words = new Choices();
            words.Add(new string[] {
                "cane",
                "abbaiare",
                "cotoletta",
                "milano",
                "freccia nera"
            });

            gb.Append(words);

            Grammar g = new Grammar( gb );
            sre.LoadGrammar(g);
            

            // start the recognition thread
            var th = new Thread( recon );
            th.Start();


            DateTime start = DateTime.Now;
            var sw = Stopwatch.StartNew();
            Console.WriteLine("Thread {0}: {1}, Priority {2}", Thread.CurrentThread.ManagedThreadId, 
                                                               Thread.CurrentThread.ThreadState, 
                                                               Thread.CurrentThread.Priority);
            do
            {
                Console.WriteLine("Thread {0}: Elapsed {1:N2} seconds", Thread.CurrentThread.ManagedThreadId, sw.ElapsedMilliseconds / 1000.0);
                Thread.Sleep(500);
            } while (sw.ElapsedMilliseconds <= 10000);
            sw.Stop();

            Thread.Sleep(1000);
            Console.WriteLine("Thread ({0}) exiting...", Thread.CurrentThread.ManagedThreadId);

        }


        private static void recon()
        {
            while( 1!=2 )
            {
                Console.WriteLine("Thread {0}: {1}, Priority {2}", Thread.CurrentThread.ManagedThreadId,
                                                                   Thread.CurrentThread.ThreadState,
                                                                   Thread.CurrentThread.Priority);
                sre.Recognize();
                //Thread.Sleep(500);
            }
        }


        public static void handler_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine("Main thread ({0}): heard {1};", Thread.CurrentThread.ManagedThreadId, e.Result.Text);
        }

        public static void handler_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            Console.WriteLine("Main thread ({0}): completed;", Thread.CurrentThread.ManagedThreadId);
        }
    }
}