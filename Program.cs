using System.Reflection;

/**
 * Tri varianty odstraneni duplicitnich hodnot z pole
 */
public class RemoveDuplicates
{

    /**
	 * Odstrani z pole prvek na indexu index
	 */
    public static int[] RemoveItem(int[] data, int index)
    {
        //vysledne pole bude o 1 kratsi
        int[] result = new int[data.Length - 1];
        //zkopirujeme prvky az do indexu i
        for (int i = 0; i < index; i++)
        {
            result[i] = data[i];
        }
        //i-ty prvek preskocime a zkopirujeme vsechny zbyvajici prvky
        for (int i = index + 1; i < data.Length; i++)
        {
            result[i - 1] = data[i];
        }
        return result;
    }

    /**
	 * Prochazi vsechny polozky a odstranuje duplikaty metodou removeItem()
	 */
    public static int[] RemoveDuplicates1(int[] data)
    {
        int[] result = data;
        for (int i = 0; i < result.Length; i++) //n
        {
            for (int j = i + 1; j < result.Length; j++) // n-1
            {
                //(n^2-n)/2
                if (result[j] == result[i])
                {
                    result = RemoveItem(result, j);
                    j--;
                }
            }
        }

        return result;
    }

    /**
	 * Prochazi vsechny polozky a provadi ostraneni vsech duplikatu jedne polozky najednou
	 */
    public static int[] RemoveDuplicates2(int[] data)
    {
        int[] result = data;
        for (int i = 0; i < result.Length; i++)
        {
            //spocteme, kolik ma polozka result[i] duplikatu
            int count = 0; //pocet duplikatu
            for (int j = i + 1; j < result.Length; j++)
            {
                if (result[j] == result[i])
                {
                    count++;
                }
            }
            //pokud je alespon jeden duplikat, pak ho odstranime
            if (count > 0)
            {
                //vysledek bude o count kratsi
                int[] newResult = new int[result.Length - count];
                //prvky az do indexu i muzeme jednoduse zkopirovat
                for (int k = 0; k <= i; k++)
                {
                    newResult[k] = result[k];
                }
                int index = i + 1; //index v cilovem poli
                for (int k = i + 1; k < result.Length; k++)
                {
                    if (result[k] != result[i])
                    { //neni duplikat
                        newResult[index] = result[k];
                        index++;
                    }
                }
                result = newResult;
            }
        }
        return result;
    }

    /**
	 * Pouziva redukci pomoci pole zaznamu, zda dane cislo bylo nalezeno v datech ci nikoli
	 */
    public static int[] RemoveDuplicates3(int[] data)
    {
        //nejdrive jen zjistime, kolik mame unikatnich cisel
        bool[] encountered = new bool[1000000];
        int count = 0; //pocet unikatnich cisel
        for (int i = 0; i < data.Length; i++)
        {
            if (!encountered[data[i]])
            { //nove objevene cislo
                encountered[data[i]] = true;
                count++;
            }
        }
        //v promenne count je ted pocet unikatnich cisel
        //pole encountered ted pouzijeme jeste jednou stejnym zpusobem
        encountered = new bool[1000000];
        int[] result = new int[count];
        int index = 0;
        for (int i = 0; i < data.Length; i++)
        {
            if (!encountered[data[i]])
            {
                result[index] = data[i];
                encountered[data[i]] = true;
                index++;
            }
        }
        return result;
    }

    /**
	 * Generuje nahodna data v rozsahu do 100 000,
	 * cimz se simuluje, ze cca 90% cisel je "neaktivnich"
	 */
    public static int[] GenerateData(int count)
    {
        int[] result = new int[count];
        Random r = new Random();
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = r.Next(100000);
        }
        return result;
    }

    /// <summary>
    /// Funkce spočítá při jaké velikosti pole <paramref name="data"/> čas na odstranění všech duplicit překročí minimální limit daný parametrem <paramref name="timeLimit"/> v milisekundách.
    /// Počáteční velikost pole je dána parametrem <paramref name="count"/> a je 10000. Pokud bude čas kratší než <paramref name="timeLimit"/>, vytvoří se nové pole o počáteční velikosti <paramref name="count"/>
    /// + její desetina. Opakuje se dokud, nepřesáhne <paramref name="timeLimit"/>. Na konci vypíše název <paramref name="testedMethod"/>, velikost pole a čas, jak dlouho běžela. 
    /// Vrací pole s časem v milisekundách a velikost pole na konci.
    /// </summary>
    /// <param name="testedMethod">Testovaná metoda</param>
    /// <param name="data">Pole celých čísel s duplicitami</param>
    /// <param name="count">Počáteční velikost pole</param>
    /// <param name="timeLimit">Minimální časový limit v milisekundách, který musí <paramref name="testedMethod"/> běžet</param>
    /// <returns>Vrací pole s časem v milisekundách a velikost pole na konci</returns>
    public static double[] TestFunction(Func<int[], int[]> testedMethod, int[] data, int count, int timeLimit)
    {
        double ms = 0.0;
        DateTime start = DateTime.Now;
        while (true)
        {
            int[] result = testedMethod(data);
            ms = (DateTime.Now - start).TotalMilliseconds;
            if (ms < timeLimit)
            {
                try
                {
                    count = count + count / 10;
                }
                catch (OverflowException ex)
                {
                    Console.WriteLine($"Běh metody \"{testedMethod.GetMethodInfo().Name}\" nemohl přesáhnout 1 sekundu z důvodu přetečení hodnoty typu Int.");
                    return [-1, -1];
                }
                
                data = GenerateData(count);
            }
            else
            {
                break;
            }
        }

        string secFormat = timeLimit / 1000 == 10 ? "sekund" : "sekundu";

        Console.WriteLine($"Metoda \"{testedMethod.GetMethodInfo().Name}\" trvala přes {timeLimit / 1000} {secFormat} (konkrétně {Math.Round(ms / 1000, 4)} s) pro n = {count}");
        return [ms, count];
    }


    /// <summary>
    /// Funkce spočítá a vrátí čas v milisekundách, jak dlouho testované metodě <paramref name="testedMethod"/> trvalo odstranit všechny duplicity z 
    /// celého pole. Testované pole se tvoří tak, že se vybere prvních <paramref name="subArraySize"/> prvků z pole <paramref name="data"/>. 
    /// </summary>
    /// <param name="testedMethod">Testovaná metoda na odstraňení duplicit</param>
    /// <param name="data">Původní pole celých čísel s duplicitami</param>
    /// <param name="subArraySize">Velikost testovaného pole</param>
    /// <returns>Vrací čas v milisekundách</returns>
    public static double TestFunction(Func<int[], int[]> testedMethod, int[] data, int subArraySize)
    {
        int[] subArray = new int[subArraySize];

        for(int i = 0; i < subArraySize; i++)
        {
            subArray[i] = data[i];
        }

        double ms = 0.0;
        DateTime start = DateTime.Now;
        int[] result = testedMethod(subArray);
        ms = (DateTime.Now - start).TotalMilliseconds;

        return ms;
    }

    /// <summary>
    /// Funkce porovná poměry (velikost pole/celkový čas) jednotlivých testovaných metod. Vrací tu metodu, jejíž poměr je největší.
    /// </summary>
    /// <param name="time1">Poměr 1. testované metody</param>
    /// <param name="time2">Poměr 2. testované metody</param>
    /// <param name="time3">Poměr 3. testované metody</param>
    /// <returns>Vrací tu metodu, jejíž poměr je největší.</returns>
    public static Func<int[], int[]> GetSlowest(double time1, double time2, double time3)
    {
        /*
        if(time1 > time2)
        {
            if(time1 > time3)
            {
                return time1;
            }
            else
            {
                return time3;
            }
        }
        else
        {
            if(time2 > time3)
            {
                return time2;
            }
            else
            {
                return time3;
            }
        }
        */

        //Tři vnořené ternární oprátory níže jsou přepsané větvění pomocí ifů nahoře
        //Má toto nějaké výhody oproti větvění if? Kromě toho, že je to úspornější na místo a nečitelné.
        Func<int[], int[]> slowest = time1 > time2 ? time1 > time3 ? RemoveDuplicates1 : RemoveDuplicates3 : time2 > time3 ? RemoveDuplicates2 : RemoveDuplicates3;
        return slowest;
    }

    public static void Main(String[] args)
    {
        int count = 10000;
        int[] data = GenerateData(count);
        
        double[] timeSize1 = TestFunction(RemoveDuplicates1, data, count, 1000);
        double[] timeSize2 = TestFunction(RemoveDuplicates2, data, count, 1000);
        double[] timeSize3 = TestFunction(RemoveDuplicates3, data, count, 1000);

        Func<int[], int[]> slowest = GetSlowest(timeSize1[0] / timeSize1[1], timeSize2[0] / timeSize2[1], timeSize3[0] / timeSize3[1]);
        Console.WriteLine($"Nejpomalejší je {slowest.GetMethodInfo().Name}");
        TestFunction(slowest, data, count, 10000);
        
        count = 1000;
        int n = 100_000;
        data = GenerateData(n);
        int lineNumber = 1;
        int step = n / 10 + count;
        string format = "{0, -4} {1, -10} | {2, -10} | {3, -10} | {4, -10} | {5, -10} | {6, -10} | {7, -10}";
        
        Console.WriteLine($"{format}", "", "n", "RemDup1", "a1", "RemDup2", "a2", "RemDup3", "a3");

        for(int i = count; i <= n; i+=step)
        {
            double time1 = TestFunction(RemoveDuplicates1, data, i) / 1000;
            double time2 = TestFunction(RemoveDuplicates2, data, i) / 1000;
            double time3 = TestFunction(RemoveDuplicates3, data, i) / 1000;
            
            slowest = GetSlowest(time1 / i, time2 / i, time3 / i);

            double slowestTime;
            if (slowest.GetMethodInfo().Name.Equals("RemoveDuplicates1"))
            {
                slowestTime = time1;
            }
            else if (slowest.GetMethodInfo().Name.Equals("RemoveDuplicates2"))
            {
                slowestTime = time2;
            }
            else
            {
                slowestTime = time3;
            }

            double a1 = slowestTime/time1;
            double a2 = slowestTime/time2;
            double a3 = slowestTime/time3;

            Console.WriteLine($"{format}", lineNumber+". ", i, Math.Round(time1, 4), Math.Round(a1, 2), Math.Round(time2, 4), Math.Round(a2, 2), Math.Round(time3, 4), Math.Round(a3, 2));
            lineNumber++;
        }

        Console.WriteLine("All done.");
    }
}