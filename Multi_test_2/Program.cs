using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Multi_test_2
{
	class Program
	{


		class Játékos
		{
			public Thread thread;
			public string Név { get; }
			public bool Vár { get => thread.ThreadState == ThreadState.Suspended; }
			public bool Kész { get => thread.ThreadState == ThreadState.Stopped; }
			Action feladat;
			public Action Feladat { 
				get => feladat;
				set 
				{
					feladat = value;
					this.thread = new Thread(new ThreadStart(feladat));
				}
			}
			public int x, y, vx, vy;

			public static List<Játékos> lista = new List<Játékos>();


			public Játékos(string Név,  int x, int y, int vx, int vy) 
			{
				this.Név = Név;
				this.x = x;
				this.y = y;
				this.vx = vx;
				this.vy = vy;				

				Játékos.lista.Add(this); 
			}

			public void Start_or_Resume()
			{
				if (thread.ThreadState == ThreadState.Suspended)
					thread.Resume();
				else if (thread.ThreadState == ThreadState.Unstarted)
					thread.Start();
			}

			public void Lép()
			{
				x += vx;
				y += vy;
				Cselekvés_vége();
			}
			public void Forog(int irány)
			{
				(vx, vy) = (vy * irány, -vx * irány);
				Cselekvés_vége();
			}

			void Cselekvés_vége()
			{
				Console.WriteLine(this);
				this.thread.Suspend();
			}

			private bool Mindenki_más_lépett_már()
			{
				foreach (Játékos játékos in Játékos.lista)
				{
					if (játékos == this)
						continue;
					if (!játékos.Vár)
						return false;
				}
				return true;
			}
			public static string Előjel(int a) => a < 0 ? "" : "+";
			public static string boolstr(bool b) => b ? "I" : "H";
			public override string ToString() => $"{Név}({x};{y}), ({Előjel(vx)}{vx}, {Előjel(vy)}{vy}), {boolstr(Vár)}{boolstr(Kész)}";

			public static void ok_elindítása()
			{
				foreach (Játékos játékos in Játékos.lista)
					if (!játékos.Kész)
						játékos.Start_or_Resume();
			}

			internal static bool ok_közül_valaki_még_nincs_kész()
			{
				foreach (Játékos játékos in Játékos.lista)
					if (!játékos.Kész)
						return true;
				return false;
			}

			internal static bool ok_mindegyike_várakozik_vagy_kész()
			{
				foreach (Játékos játékos in Játékos.lista)
					if (!(játékos.Vár||játékos.Kész))
					{
						Console.WriteLine($"{játékos} még nem várakozik!");
						return false;
					}
				return true;
			}
		}

		/*
		static void Vár_amíg_nincs_kész_mindenki(Résztvevő játékvezető)
		{
			while (!játékvezető.Kész)
			{
				Console.WriteLine("Főszál: a játékvezető még nincs kész.");
				foreach (Játékos játékos in Játékos.lista)
				{
					Console.Write($"{játékos} | ");
				}
				Console.WriteLine(Játékos.játékvezető);
				Thread.Sleep(10000);
			}
			Console.WriteLine("Főszál: a játékvezető készen van.");
		}
		*/

		static int balra = -1;
		static int jobbra = 1;

		static void Main(string[] args)
		{
			Console.WriteLine("Itt most az lesz, hogy egy játékvezető egyszerre lépteti a játékosokat. " +
				"A játékvezető szól, hogy lépjenek, ezalatt elmegy aludni. " +
				"A többiek lépnek, és mikor már mindenki lépett, felébresztik a játékvezetőt, aki elrendezi a pályamódosításokat.");

			Console.WriteLine("---------------------\nElőkészületek");

			Játékos karesz = new Játékos("Karesz", 0, 0, 1, 0);
			Játékos lilesz = new Játékos("Lilesz", 1, 1, 0, 1);
			Játékos janesz = new Játékos("Janesz", 0, 1, 1, -1);

			Console.WriteLine("---------------------\nJátékosok programozása");



			karesz.Feladat = delegate () 
			{
				for (int i = 0; i < 5; i++)
				{
					karesz.Lép();
				}
			};

			lilesz.Feladat = delegate ()
			{
				for (int i = 0; i < 8; i++)
				{
					lilesz.Lép();
					lilesz.Lép();
					lilesz.Forog(jobbra);
				}
			};

			janesz.Feladat = delegate ()
			{
				janesz.Lép();
				janesz.Lép();
				janesz.Lép();
				for (int i = 0; i < 2; i++)
				{
					janesz.Forog(jobbra);
				}
				janesz.Lép();
				janesz.Lép();
			};

			Console.WriteLine("---------------------\nKörök");
			
			Játékos.ok_elindítása();

			int várakozási_idő = 1000;

			Thread.Sleep(várakozási_idő);
			while (Játékos.ok_közül_valaki_még_nincs_kész())
			{
//				Console.WriteLine("Játékvezető: A játékosok közül valaki még nincs kész.");
				if (Játékos.ok_mindegyike_várakozik_vagy_kész())
				{
	//				Console.WriteLine("Játékvezető: A játékosok mind rám várnak.");
					Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - ");
					Játékos.ok_elindítása();
				}
				else
				{
		//			Console.WriteLine("Játékvezető: A játékosok valamelyike nem várakozik.");
				}
				Thread.Sleep(várakozási_idő);
			}
			//Console.WriteLine("Játékvezető: A játékosok közül már mindenki készen van.");



			Console.WriteLine("Nyomj meg egy gombot, hogy vége legyen.");
			Console.ReadKey();
		}
	}
}
/*
 */