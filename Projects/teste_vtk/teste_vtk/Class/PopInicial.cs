using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using teste_vtk.Class;

//Tipos de empenagem
//1- Cauda Convencional
//2- Cauda em H
//3- Cauda em U
//4-Cauda em V

//Tipos de asa
//2-Asa Mista
//1-Asa Retangular
//0-Asa Trapedoizal
//3-Asa Voadora

namespace teste_vtk
{
    class PopInicial
    {
        Random rand = new Random();
        Avaliacao avalia = new Avaliacao();
        Parametros param = new Parametros();

        const int Populacao = 78;
        public double[,] pop = new double[15, 80];
        public double[,] filho = new double[15, 80];

            //0-alongamento da asa
            //1-incidencia da asa
            //2-afilamento da asa
            //3-altura da asa
            //4-distancia inicial entre o cg e o centro aerodinâmico (asa)
            //5-variação da envergadura da asa
            //6-Coeficiente de sustentação da asa
            //7-Tipo da Asa
            //8-Tipo da empenagem
            //9-Braço da empenagem horizontal
            //10-Volume de cauda Horizontal
            //11-Alongamento de empenagem Horizontal
            //12-Angulo de incidencia da EH
            //13-Braço da empenagem vertical
            //14-pontuacao

        

        public double GeraAleatorioDouble(double min, double max)
        {
            double next = rand.NextDouble();
            return min + (next * (max - min));
        }

        public int GeraAleatorioInt(int min, int max)
        {
            return rand.Next(min, max);
        }
        public void GeraPopIniciaL()
        {
            int i=0, cauda=0;
            double pontuacao;

            while (i < Populacao)
            {
                cauda++;
                pop[0, i] = Math.Round(GeraAleatorioDouble(param.arMin, param.arMax), 1);
                pop[1, i] = Math.Round(GeraAleatorioDouble(param.iwMin, param.iwMax), 1);
                pop[2, i] = Math.Round(GeraAleatorioDouble(param.alMin, param.alMax), 1);
                pop[3, i] = Math.Round(GeraAleatorioDouble(param.hMin, param.hMax), 3);
                pop[4, i] = Math.Round(GeraAleatorioDouble(param.xcgacMin, param.xcgacMax), 2);
                pop[5, i] = Math.Round(GeraAleatorioDouble(param.xwMin, param.xwMax), 2);
                pop[6, i] = Math.Round(GeraAleatorioDouble(param.cl_asaMin,param.cl_asaMax), 2);
                System.Console.WriteLine(pop[6,i]);
                if (pop[2, i] == 1)
                {
                    pop[7, i] = 1;
                }
                else
                {
                    if (pop[5, i] == 0)
                        pop[7, i] = 0;
                    else
                        if (pop[5, i] > 0 && pop[5, i] < 1)
                            pop[7, i] = 2;
                        else
                            if (pop[5, i] == 1)
                                pop[7, i] = 1;

                }
                 //tipo empenagem
                if (cauda == 4)
                    cauda = 1;
                pop[8, i] = cauda;
                //empenagem horizontal
                pop[9, i] = Math.Round(GeraAleatorioDouble(param.lht_ehMin, param.lht_ehMax), 3);
                pop[10, i] = Math.Round(GeraAleatorioDouble(param.vlhMin, param.vlhMax), 3);
                pop[11, i] = Math.Round(GeraAleatorioDouble(param.ar_ehMin, param.ar_ehMax), 1);
                pop[12, i] = Math.Round(GeraAleatorioDouble(param.itMin, param.itMax), 1);

                //empenagem verticalpop[13, i] = Math.Round(GeraAleatorioDouble(param.lht_evMin, param.lht_evMax), 3);


                pontuacao  = avalia.calculos(pop[1, i], pop[4, i], pop[0, i], pop[2, i], pop[3, i], pop[5, i], pop[9, i], pop[10, i], pop[11, i], pop[12, i], pop[13, i], pop[6, i], pop[7, i],pop[8, i],false, false);
                if (pontuacao > 0)
                {
                    pop[14, i] = pontuacao;
                    i++;
                    
                }

            }
            
        }
        public int seleciona_aviao()
        {
            double wmaior = 0;
            int i, windice = 81;
            for (i = 0; i < 80; i++)
            {
                if (pop[14, i] > wmaior)
                {
                    wmaior = pop[14, i];
                    windice = i;
                }
            }
            return windice;
        }

        public int seleciona_pais()
        {
            double soma = 0, aux = 0, s = 0;
            int i;
            for (i = 0; i < 80; i++)
            {
                soma = soma + pop[14, i];
            }
            s = GeraAleatorioDouble(0, soma);
            i = 0;
            aux = pop[14, i];
            while (aux < s)
            {
                i++;
                aux = aux+pop[14, i];
            }
            return i;
        }

        public void crossover(int pai1, int pai2, int f)
        {
            int CorteAsa, CorteEmp,i;
            CorteAsa = GeraAleatorioInt(0, 7);
            CorteEmp = GeraAleatorioInt(8, 13);
            for (i = 0; i < CorteAsa; i++)
            {
                filho[i, f] = pop[i, pai1];
                filho[i, f + 1] = pop[i, pai2];
            }
            for (i = CorteAsa; i < 8; i++)
            {
                filho[i, f] = pop[i, pai2];
                filho[i, f + 1] = pop[i, pai1];
            }
            for (i = 8; i < CorteEmp; i++)
            {
                filho[i, f] = pop[i, pai1];
                filho[i, f + 1] = pop[i, pai2];
            }
            for (i = CorteEmp; i < 14; i++)
            {
                filho[i, f] = pop[i, pai2];
                filho[i, f + 1] = pop[i, pai1];
            }
        }

        public double intervalo_mutacao(double valor, int i)
        {
            if (i == 0 && param.arMax < valor)
                return param.arMax;
            else
                if (i == 1 && param.iwMax < valor)
                    return param.iwMax;
            else
                if (i == 2 && param.alMax < valor)
                    return param.alMax;
                else
                    if (i == 3 && param.hMax < valor)
                        return param.hMax;
                    else
                        if (i == 4 && param.xcgacMax < valor)
                            return param.xcgacMax;
                        else
                            if (i == 5 && param.xwMax < valor)
                                return param.xwMax;
                            else
                                if (i == 6 && param.cl_asaMax < valor)
                                    return param.cl_asaMax;
                                else
                                    if (i == 9 && param.lht_ehMax < valor)
                                        return param.lht_ehMax;
                                    else
                                        if (i == 10 && param.vlhMax < valor)
                                            return param.vlhMax;
                                        else
                                            if (i == 11 && param.ar_ehMax < valor)
                                                return param.ar_ehMax;
                                            else
                                                if (i == 12 && param.itMax < valor)
                                                    return param.itMax;
                                                else
                                                   if (i == 13 && param.lht_evMax < valor)
                                                       return param.lht_evMax;
                                                    else
                                                        return valor;
        }
        public void mutacao(int i)
        { 
            int j=7;
            double p, prob=0.05;
            p = GeraAleatorioDouble(0, 1);
            if (p < prob)
            {
                while(j==7 || j==8)
                    j = GeraAleatorioInt(0, 13);
                filho[j, i] = filho[j, i] + filho[j, i] * 0.01;
                filho[j, i]=intervalo_mutacao(filho[j, i], j);
                if (j == 2)
                {
                    if (filho[j, i] == 1)
                    {
                        filho[7, i] = 1;
                    }
                }
            }
        }

        public void insercao(int i)
        {
            int r=2;//elite
            int j, k, h;
            for (h = 0; h < r; h++)
            {
                j = seleciona_aviao();
                for (k = 0; k < 15; k++)
                {
                    filho[k, i] = pop[k, j];
                    pop[k, j] = 0;
                }
                i++;
            }
            for (i = 0; i < 76; i++)
            {
                for (j = 0; j < 15; j++)
                {
                    pop[j, i] = filho[j, i];
                    filho[j, i] = 0;
                }
            }  
        }

        internal delegate void UpdateProgressDelegate(int ProgressPercentage);
        internal event UpdateProgressDelegate UpdateProgress;
        internal void DoSomething()
        {
            for (int i = 0; i <= 100; i++)
            {
                System.Threading.Thread.Sleep(100);
                UpdateProgress(i);
                Application.DoEvents();
            }
        }

        public void main(Parametros param, int wgeracoes)
        {
            int g=0, p1=0, p2=0, i=0;
            double pontuacao1 = 0, pontuacao2=0;
            GeraPopIniciaL();
            for (g = 1; g < wgeracoes;g++)
            {
                i = 0;
                while (i < 76)
                {
                    p1 = seleciona_pais();
                    p2 = p1;
                    while(p1==p2)
                    {
                        p2 = seleciona_pais();
                    }
                    crossover(p1, p2, i);
                    mutacao(i);
                    mutacao(i+1);
                    pontuacao1 = avalia.calculos(filho[1, i], filho[4, i], filho[0, i], filho[2, i], filho[3, i], filho[5, i], filho[9, i], filho[10, i], filho[11, i], filho[12, i], filho[13, i], filho[6, i], filho[7, i], filho[8, i], false, false);
                    pontuacao2 = avalia.calculos(filho[1, i + 1], filho[4, i + 1], filho[0, i + 1], filho[2, i + 1], filho[3, i + 1], filho[5, i + 1], filho[9, i + 1], filho[10, i + 1], filho[11, i + 1], filho[12, i + 1], filho[13, i + 1], filho[6, i + 1], filho[7, i+1], filho[8, i+1], false, false);

                    if (pontuacao1 > 0 && pontuacao2 > 0)
                    {
                        filho[14, i] = pontuacao1;
                        filho[14, i + 1] = pontuacao2;
                        i=i+2;
                    }  
                    
                }
                insercao(i);
                UpdateProgress(g);
                Application.DoEvents();
            }
        }
    }
}
