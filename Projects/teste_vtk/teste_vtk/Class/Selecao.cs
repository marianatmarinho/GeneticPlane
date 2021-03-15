using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace teste_vtk
{
    class Selecao
    {
        PopInicial Popini = new PopInicial();

        public void seleciona_pais(double[,] p)
        {
            double soma = 0, aux = 0, s = 0;
            int i;
            
            for (i = 0; i < 80; i++)
            {
                soma += p[14, i];
            }
            s = Popini.GeraAleatorioDouble(0, soma);
            i = 0;
            aux = p[14, i];
            while (aux < s)
            {
                i++;
                aux = +p[14, i];
            }
        }

    }
}
