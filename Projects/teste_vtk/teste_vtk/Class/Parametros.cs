using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace teste_vtk.Class
{
    class Parametros
    {
       public double ar_min = 4;
       public double ar_max = 8;
    //1-incidencia da asa
       public double iw_min = -2;
       public double iw_max = 6;
    //2-afilamento da asa
       public double al_min = 0.3;
       public double al_max = 1;
    //3-altura da asa
       public double h_min = 0.08;
       public double h_max = 0.15;
    //4-distancia inicial entre o cg e o centro aerodinâmico (asa)
       public double xcgac_min = -0.2;
       public double xcgac_max = 0.2;
    //5-variação da envergadura da asa
       public double xw_min = 0;
       public double xw_max = 1;
    //6-Coeficiente de sustentação da asa
       public double cl_asa_min = 1.8;
       public double cl_asa_max = 4;
    //7-Tipo da Asa
    //8-Tipo da empenagem
    //9-Braço da empenagem horizontal
       public double lht_eh_min = 0.4;
       public double lht_eh_max = 0.7;
    //10-Volume de cauda Horizontal
       public double vlh_min = 0.3;
       public double vlh_max = 0.5;
    //11-Coeficiente de sustentação empenagem horizontal
       public double cl_eh_min = 1;
       public double cl_eh_max = 1.6;
    //12-Alongamento de empenagem Horizontal
       public double ar_eh_min = 3;
       public double ar_eh_max = 4;
    //13-Angulo de incidencia da EH
        double it_min = -2;
        double it_max = 6;
    //14-Volume de Cauda vertical
        public double vlv_min = 0.04;
        public double vlv_max = 0.06;
    //15-Coeficiente de Sustentação para empenagem vertical e suas variações de perfis
        public double cl_ev_min = 1;
        public double cl_ev_max = 1.4;
    //16-Alongamento de empenagem vertical
        public double ar_ev_min = 2;
        public double ar_ev_max = 3;
    //17-Braço da empenagem vertical
        public double lht_ev_min = 0.4;
        public double lht_ev_max = 0.7;

        public void Limpar()
        {
            ar_min = 4;
            ar_max = 8;
            //1-incidencia da asa
            iw_min = -2;
            iw_max = 6;
            //2-afilamento da asa
            al_min = 0.3;
            al_max = 1;
            //3-altura da asa
            h_min = 0.08;
            h_max = 0.15;
            //4-distancia inicial entre o cg e o centro aerodinâmico (asa)
            xcgac_min = -0.2;
            xcgac_max = 0.2;
            //5-variação da envergadura da asa
            xw_min = 0;
            xw_max = 1;
            //6-Coeficiente de sustentação da asa
            cl_asa_min = 1.8;
            cl_asa_max = 4;
            //7-Tipo da Asa
            //8-Tipo da empenagem
            //9-Braço da empenagem horizontal
            lht_eh_min = 0.4;
            lht_eh_max = 0.7;
            //10-Volume de cauda Horizontal
            vlh_min = 0.3;
            vlh_max = 0.5;
            //11-Coeficiente de sustentação empenagem horizontal
            cl_eh_min = 1;
            cl_eh_max = 1.6;
            //12-Alongamento de empenagem Horizontal
            ar_eh_min = 3;
            ar_eh_max = 4;
            //13-Angulo de incidencia da EH
            it_min = -2;
            it_max = 6;
            //14-Volume de Cauda vertical
            vlv_min = 0.04;
            vlv_max = 0.06;
            //15-Coeficiente de Sustentação para empenagem vertical e suas variações de perfis
            cl_ev_min = 1;
            cl_ev_max = 1.4;
            //16-Alongamento de empenagem vertical
            ar_ev_min = 2;
            ar_ev_max = 3;
            //17-Braço da empenagem vertical
            lht_ev_min = 0.4;
            lht_ev_max = 0.7;
        }

        public double arMin
        {
            get { return ar_min; }
            set { ar_min = value; }
        }
  
        public double arMax
        {
            get { return ar_max; }
            set { ar_max = value; }
        }

        public double iwMin
        {
            get { return iw_min; }
            set { iw_min = value; }
        }

        public double iwMax
        {
            get { return iw_max; }
            set { iw_max = value; }
        }

        public double alMin
        {
            get { return al_min; }
            set { al_min = value; }
        }

        public double alMax
        {
            get { return al_max; }
            set { al_max = value; }
        }

        public double hMin
        {
            get { return h_min; }
            set { h_min = value; }
        }

        public double hMax
        {
            get { return h_max; }
            set { h_max = value; }
        }

        public double xcgacMin
        {
            get { return xcgac_min; }
            set { xcgac_min = value; }
        }

        public double xcgacMax
        {
            get { return xcgac_max; }
            set { xcgac_max = value; }
        }

        public double xwMin
        {
            get { return xw_min; }
            set { xw_min = value; }
        }

        public double xwMax
        {
            get { return xw_max; }
            set { xw_max = value; }
        }

        public double cl_asaMin
        {
            get { return cl_asa_min; }
            set { cl_asa_min = value; }
        }

        public double cl_asaMax
        {
            get { return cl_asa_max; }
            set { cl_asa_max = value; }
        }

        public double lht_ehMin
        {
            get { return lht_eh_min; }
            set { lht_eh_min = value; }
        }

        public double lht_ehMax
        {
            get { return lht_eh_max; }
            set { lht_eh_max = value; }
        }

        public double vlhMin
        {
            get { return vlh_min; }
            set { vlh_min = value; }
        }

        public double vlhMax
        {
            get { return vlh_max; }
            set { vlh_max = value; }
        }

         public double cl_ehMin
        {
            get { return cl_eh_min; }
            set { cl_eh_min = value; }
        }

        public double cl_ehMax
        {
            get { return cl_eh_max; }
            set { cl_eh_max = value; }
        }

        public double ar_ehMin
        {
            get { return ar_eh_min; }
            set { ar_eh_min = value; }
        }

        public double ar_ehMax
        {
            get { return ar_eh_max; }
            set { ar_eh_max = value; }
        }

        public double itMin
        {
            get { return it_min; }
            set { it_min = value; }
        }

        public double itMax
        {
            get { return it_max; }
            set { it_max = value; }
        }

        public double vlvMin
        {
            get { return vlv_min; }
            set { vlv_min = value; }
        }

        public double vlvMax
        {
            get { return vlv_max; }
            set { vlv_max = value; }
        }

        public double cl_evMin
        {
            get { return cl_ev_min; }
            set { cl_ev_min = value; }
        }

        public double cl_evMax
        {
            get { return cl_ev_max; }
            set { cl_ev_max = value; }
        }
        public double ar_evMin
        {
            get { return ar_ev_min; }
            set { ar_ev_min = value; }
        }

        public double ar_evMax
        {
            get { return ar_ev_max; }
            set { ar_ev_max = value; }
        }
        public double lht_evMin
        {
            get { return lht_ev_min; }
            set { lht_ev_min = value; }
        }

        public double lht_evMax
        {
            get { return lht_ev_max; }
            set { lht_ev_max = value; }
        }
    }
}
