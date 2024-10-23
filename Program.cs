using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElectionsSimulation.Program;

namespace ElectionsSimulation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //format - Stát,Počet hlasů,Počet voličů,Koeficient účasti,Koeficient pro demokraty,Koeficient pro republikány
            Console.ForegroundColor = ConsoleColor.White;
            StreamReader sr = new StreamReader("Dataset.csv");
            string line = sr.ReadLine();
            states states = new states();
            states.StatesList = new List<state>();

            while (line != null)
            {
                string[] parts = line.Split(';');
                state stat = new state();
                stat.Name = parts[0];
                stat.Votes = int.Parse(parts[1]);
                stat.Voters = int.Parse(parts[2]);
                stat.Participation = double.Parse(parts[3]);
                stat.Democrats = double.Parse(parts[4]);
                stat.Republicans = double.Parse(parts[5]);
                states.StatesList.Add(stat);
                line = sr.ReadLine();
                stat.VoteCount(stat, states);
            }
            sr.Close();
            if (states.SumDemVotes > states.SumRepVotes)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"Vítězem prezidenstých voleb je KAMALA HARRIS s celkovým počtem hlasů {states.SumDemVotes} ({states.VotersForDem})!");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (states.SumDemVotes < states.SumRepVotes)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Vítězem prezidenstých voleb je DONALD TRUMP s celkovým počtem hlasů {states.SumRepVotes} ({states.VotersForRep})!");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.WriteLine($"Je to remíza! (pro demokraty{states.SumDemVotes} a pro republikány {states.SumRepVotes})");
            }
            Console.WriteLine();
            Console.WriteLine("Pro Demokraty hlasovalo: " + states.VotersForDem + " voličů");
            Console.WriteLine("Pro Republikány hlasovalo: " + states.VotersForRep + " voličů");
            Console.WriteLine("Celkově hlasovalo " + states.SumVoters + " voličů");
            Console.ReadLine();
            Console.WriteLine("Zadejte QUIT pro ukončení programu");
            while (true)
            {
                if (Console.ReadLine().ToUpper() == "QUIT")
                {
                    break;
                }
            }
        }
        public class states
        {
            public List<state> StatesList { get; set; }
            public int SumDemVotes = 0;
            public int SumRepVotes = 0;
            public int SumVoters = 0;
            public int VotersForDem = 0;
            public int VotersForRep = 0;
            public void addVotesDem(state state)
            {
                SumDemVotes += state.Votes;
            }
            public void addVotesRep(state state)
            {
                SumRepVotes += state.Votes;
            }
        }

        public class state
        {
            public string Name { get; set; }
            public int Votes { get; set; }
            public int Voters { get; set; }
            public double Participation { get; set; }
            public double Democrats { get; set; }
            public double Republicans { get; set; }
            public int DemVoters { get; set; }
            public int RepVoters { get; set; }
            public double ActualVoters { get; set; }

            public void WriteState(state state)
            {
                double ActualParticipation = ActualVoters / Voters * 100;
                Console.WriteLine("Stát: " + state.Name);
                Console.WriteLine("Počet hlasů: " + state.Votes);
                Console.WriteLine("Celkový počet voličů: " + state.Voters);
                Console.WriteLine($"Účast: {Math.Round(ActualParticipation, 2)}% ({ActualVoters})");
                Console.WriteLine("Hlasů pro demokraty: " + DemVoters);
                Console.WriteLine("Hlasů pro Republikány: " + RepVoters);
                Console.WriteLine();
            }

            public void VoteCount(state state, states states)
            {   
                state.ActualVoters = (int)(state.Voters * state.Participation * Variability());
                int demVoters = (int)(state.ActualVoters * state.Democrats * Variability());
                
                int repVoters = (int)state.ActualVoters - demVoters;

                DemVoters = demVoters;
                RepVoters = repVoters;
                states.SumVoters += DemVoters + RepVoters;
                states.VotersForDem += DemVoters;
                states.VotersForRep += RepVoters;
                
                WriteState(state);
                if (DemVoters > RepVoters)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Kamala Harris vyhrála ve státě " + state.Name);
                    Console.ForegroundColor = ConsoleColor.White;
                    states.addVotesDem(state);
                }
                else if (DemVoters < RepVoters)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Donald Trump vyhrál ve státě " + state.Name);
                    Console.ForegroundColor = ConsoleColor.White;
                    states.addVotesRep(state);
                }
                else
                {
                    Console.WriteLine("Je to remíza ve státě " + state.Name);
                }
                Console.WriteLine();
                Console.WriteLine("Počet hlasů pro Demokraty: " + states.SumDemVotes);
                Console.WriteLine("Počet hlasů pro Republikány: " + states.SumRepVotes);
                Console.ReadLine();
                Console.WriteLine("----------------------------------");
            }
            private double Variability()
            {
                Random rnd = new Random();
                double variability = rnd.Next(-30, 30);
                variability *= 0.01;
                variability = 1 - variability;
                return variability;
            }
        }
    }
}
