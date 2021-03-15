using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//VTK
using Kitware.VTK;
using System.Globalization;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using NumpyDotNet;
using clr_mtrand;
using teste_vtk.Class;
using System.IO;

namespace teste_vtk
{
    public partial class Form_simulacao : Form
    {
        #region Variaveis
        PopInicial Popini = new PopInicial();
        Parametros param = new Parametros();

        int wmaior_pontuacao;
        string wusu;
        public string wfilename;

        static vtkSTLReader sphere;
        static vtkPolyDataMapper mapper;
        static vtkRenderer ren1=vtkRenderer.New();
        static vtkRenderWindow renWin;
        vtkLODActor actor = vtkLODActor.New();
        static class Constants
        {
            //0-alongamento da asa
            public const double ar_min = 4;
            public const double ar_max = 8;
            //1-incidencia da asa
            public const double iw_min = -2;
            public const double iw_max = 6;
            //2-afilamento da asa
            public const double al_min = 0.3;
            public const double al_max = 1;
            //3-altura da asa
            public const double h_min = 0.08;
            public const double h_max = 0.15;
            //4-distancia inicial entre o cg e o centro aerodinâmico (asa)
            public const double xcgac_min = -0.2;
            public const double xcgac_max = 0.2;
            //5-variação da envergadura da asa
            public const double xw_min = 0;
            public const double xw_max = 1;
            //6-Coeficiente de sustentação da asa
            public const double cl_asa_min = 1.8;
            public const double cl_asa_max = 4;
            //7-Tipo da Asa
            //8-Tipo da empenagem
            //9-Braço da empenagem horizontal
            public const double lht_eh_min = 0.4;
            public const double lht_eh_max = 0.7;
            //10-Volume de cauda Horizontal
            public const double vlh_min = 0.3;
            public const double vlh_max = 0.5;
            //11-Coeficiente de sustentação empenagem horizontal
            public const double cl_eh_min = 1;
            public const double cl_eh_max = 1.6;
            //12-Alongamento de empenagem Horizontal
            public const double ar_eh_min = 3;
            public const double ar_eh_max = 4;
            //13-Angulo de incidencia da EH
            public const double it_min = -2;
            public const double it_max = 6;
            //14-Volume de Cauda vertical
            public const double vlv_min = 0.04;
            public const double vlv_max = 0.06;
            //15-Coeficiente de Sustentação para empenagem vertical e suas variações de perfis
            public const double cl_ev_min = 1;
            public const double cl_ev_max = 1.4;
            //16-Alongamento de empenagem vertical
            public const double ar_ev_min = 2;
            public const double ar_ev_max = 3;
            //17-Braço da empenagem vertical
            public const double lht_ev_min = 0.4;
            public const double lht_ev_max = 0.7;

        }
        #endregion 
        
        #region Construtor
        public Form_simulacao(string wusu_id)
        {
            
            InitializeComponent();
            inicializa_parametros();
            renderWindowControl1.Visible = false;
            btn_export.Enabled = false;
            wusu = wusu_id;
            pBar1.Visible = false;
            pbar_label.Visible = false;
            Popini.UpdateProgress += UpdateProgress;
            
          /*  foreach (Parametros p in param)
            {
                p.al_max = 1; 
            }*/

        }

        public Form_simulacao()
        {
            InitializeComponent();
            inicializa_parametros();
            renderWindowControl1.Visible = false;
            btn_export.Enabled = false;
            pBar1.Visible = false;
            pbar_label.Visible = false;
            Popini.UpdateProgress += UpdateProgress;
        }
        #endregion

        public void start_progress()
        {
            pBar1.Visible = true;
            pBar1.Minimum = 0;
            pBar1.Maximum = int.Parse(txt_geracoes.Text)-1;
        }
        private void UpdateProgress(int ProgressPercentage)
        {
            pBar1.Value = ProgressPercentage;
        }
        public void SetProgress()
        {
            pBar1.Increment(10);
        }

        public void IncrementProgress()
        {
            SetProgress();
        }
 
        public event EventHandler ThresholdReached;

        protected virtual void OnThresholdReached(EventArgs e)
        {
            EventHandler handler = ThresholdReached;
            if (handler != null)
            {
                handler(this, e);
            }
        }

       
         public void renderWindowControl1_Load()
         {
             ren1.RemoveActor(actor);
             // Create a simple sphere. A pipeline is created.
             sphere = vtkSTLReader.New();
             // sphere.SetFileName("../../../ret.stl");
             //sphere.SetFileName("../../../teste_vtk/bin/Debug/cr001sm CFD.stl");
             sphere.SetFileName(wfilename);

             mapper = vtkPolyDataMapper.New();
             mapper.SetInputConnection(sphere.GetOutputPort());


             actor.SetOrientation(-60,15,45);
             actor.SetMapper(mapper);
             //actor.RotateX(30.0);
             //actor.RotateY(-45.0);
             /*actor.RotateX(-60.0);
             actor.RotateY(15.0);
             actor.RotateZ(45);*/
             //ren1 = vtkRenderer.New();
             ren1.AddViewProp(actor);
             
            
             renderWindowControl1.Load += WindowOnLoad;

         }

         public void inicializa_parametros()
         {
             param.Limpar();
             //0-alongamento da asa
             txt_ar_min.Text = param.arMin.ToString();
             txt_ar_max.Text = param.arMax.ToString();
             //1-incidencia da asa
             txt_iw_min.Text = param.iwMin.ToString();
             txt_iw_max.Text = param.iwMax.ToString();
             //2-afilamento da asa
             txt_al_min.Text = param.alMin.ToString();
             txt_al_max.Text = param.alMax.ToString();
             //3-altura da asa
             txt_h_min.Text = param.hMin.ToString();
             txt_h_max.Text = param.hMax.ToString();
             //4-distancia inicial entre o cg e o centro aerodinâmico (asa)
             txt_xcgac_min.Text = param.xcgacMin.ToString();
             txt_xcgac_max.Text = param.xcgacMax.ToString();
             //5-variação da envergadura da asa
             txt_xw_min.Text = param.xwMin.ToString();
             txt_xw_max.Text = param.xwMax.ToString();
             //6-Coeficiente de sustentação da asa
             txt_cl_asa_min.Text = param.cl_asaMin.ToString();
             txt_cl_asa_max.Text = param.cl_asaMax.ToString();
             //7-Tipo da Asa
             //8-Tipo da empenagem
             //9-Braço da empenagem horizontal
             txt_lht_eh_min.Text = param.lht_evMin.ToString();
             txt_lht_eh_max.Text = param.lht_ehMax.ToString();
             //10-Volume de cauda Horizontal
             txt_vlh_min.Text = param.vlhMin.ToString();
             txt_vlh_max.Text = param.vlhMax.ToString();
             //11-Coeficiente de sustentação empenagem horizontal
             //txt_cl_eh_min.Text = Constants.cl_eh_min.ToString();
             //txt_cl_eh_max.Text = Constants.cl_eh_max.ToString();
             ////12-Alongamento de empenagem Horizontal
             txt_ar_eh_min.Text = param.ar_ehMin.ToString();
             txt_ar_eh_max.Text = param.ar_ehMax.ToString();
             //13-Angulo de incidencia da EH
             txt_it_min.Text = param.itMin.ToString();
             txt_it_max.Text = param.itMax.ToString();
             //14-Volume de Cauda vertical
             //txt_vlv_min.Text = Constants.vlv_min.ToString();
             //txt_vlv_max.Text = Constants.vlv_max.ToString();
             //15-Coeficiente de Sustentação para empenagem vertical e suas variações de perfis
             //txt_cl_ev_min.Text = Constants.cl_ev_min.ToString();
             //txt_cl_ev_max.Text = Constants.cl_ev_max.ToString();
             //16-Alongamento de empenagem vertical
             //txt_ar_ev_min.Text = Constants.ar_ev_min.ToString();
             //txt_ar_ev_max.Text = Constants.ar_ev_max.ToString();
             //17-Braço da empenagem vertical
             txt_lht_ev_min.Text = param.lht_evMin.ToString();
             txt_lht_ev_max.Text = param.lht_evMax.ToString();
             txt_simulador.Text = string.Empty;
             txt_pontuacao_result.Text = string.Empty;
             txt_geracoes.Text = string.Empty;
             txt_ar_result.Text = string.Empty;
             txt_iw_result.Text = string.Empty;
             txt_al_result.Text = string.Empty;
             txt_h_result.Text = string.Empty;
             txt_xcgac_result.Text = string.Empty;
             txt_xw_result.Text = string.Empty;
             txt_cl_asa_result.Text = string.Empty;
             txt_lht_eh_result.Text = string.Empty;
             txt_vlh_result.Text = string.Empty;
             txt_ar_eh_result.Text = string.Empty;
             txt_it_result.Text = string.Empty;
             txt_lht_ev_result.Text = string.Empty;
             txt_tipo_asa_result.Text = string.Empty;
             txt_tipo_emp_result.Text = string.Empty;
             txt_tipo_emp_id.Text = string.Empty;
             txt_tipo_asa_id.Text = string.Empty;
             txt_ar_min.Focus();

         }

         
         public void mostra_resultado()
         {
             //1- Cauda Convencional
             //2- Cauda em H
             //3- Cauda em U
             //4-Cauda em V

             //Tipos de asa
             //2-Asa Mista
             //1-Asa Retangular
             //0-Asa Trapedoizal
             //3-Asa Voadora
             double[,] p = Popini.pop;
             txt_ar_result.Text = p[0, wmaior_pontuacao].ToString();
             txt_iw_result.Text = p[1, wmaior_pontuacao].ToString();
             txt_al_result.Text=p[2,wmaior_pontuacao].ToString();
             txt_h_result.Text=p[3,wmaior_pontuacao].ToString();
             txt_xcgac_result.Text=p[4,wmaior_pontuacao].ToString();
             txt_xw_result.Text=p[5,wmaior_pontuacao].ToString();
             txt_cl_asa_result.Text=p[6,wmaior_pontuacao].ToString();
             txt_tipo_asa_id.Text = p[7, wmaior_pontuacao].ToString();
             if (p[7, wmaior_pontuacao] == 0)
             {
                 txt_tipo_asa_result.Text = "Trapezoidal";
             }
             else
             {
                 if (p[7, wmaior_pontuacao] == 1)
                     txt_tipo_asa_result.Text = "Retangular";
                 else
                     if (p[7, wmaior_pontuacao] == 2)
                         txt_tipo_asa_result.Text = "Mista";
                     else
                         if (p[7, wmaior_pontuacao] == 3)
                             txt_tipo_asa_result.Text = "Voadora";
             }
             txt_tipo_emp_id.Text = p[8, wmaior_pontuacao].ToString();
             if (p[8, wmaior_pontuacao] == 1)
             {
                 txt_tipo_emp_result.Text = "Convencional";
             }
             else
             {
                 if (p[8, wmaior_pontuacao] == 2)
                     txt_tipo_emp_result.Text = "em H";
                 else
                     if (p[8, wmaior_pontuacao] == 3)
                         txt_tipo_emp_result.Text = "em U";
                     else
                         if (p[8, wmaior_pontuacao] == 4)
                             txt_tipo_emp_result.Text = "em V";
             }
             txt_lht_eh_result.Text=p[9,wmaior_pontuacao].ToString();
             txt_vlh_result.Text=p[10,wmaior_pontuacao].ToString();
             txt_ar_eh_result.Text=p[11,wmaior_pontuacao].ToString();
             txt_it_result.Text=p[12,wmaior_pontuacao].ToString();
             txt_lht_ev_result.Text = p[13, wmaior_pontuacao].ToString();
             txt_pontuacao_result.Text = Math.Round(p[14, wmaior_pontuacao],2).ToString();
         }


         private void WindowOnLoad(object sender, EventArgs e)
         {
             renWin = vtkRenderWindow.New();
             ren1 = renderWindowControl1.RenderWindow.GetRenderers().GetFirstRenderer();
             renWin = renderWindowControl1.RenderWindow;

             // Add the actors to the renderer, set the window size
             //
             ren1.AddViewProp(actor);
             renWin.SetSize(250, 250);
             renWin.Render();
             vtkCamera camera = ren1.GetActiveCamera();
             camera.Zoom((double)1.5);
         }

        private void Valida_Campo_Numerico_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ',') && (e.KeyChar != '-'))
            {
                e.Handled = true;
                MessageBox.Show("São permitidos apenas valores númericos!", "Atenção!");
            }
        }

        private void txt_geracoes_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("São permitidos apenas valores númericos e inteiros!", "Atenção!");
            }
        }
        private void valida_intervalo(CancelEventArgs e, double wmin, double wmax, double wvalor_min, double wvalor_max )
        {
            if (!(wvalor_min >= wmin && wvalor_min <= wmax) || (!(wvalor_max >= wmin && wvalor_max <= wmax)))
            {
                e.Cancel=true;
                MessageBox.Show("São permitidos apenas valores entre "+wmin+" e "+wmax+"!", "Atenção!");
            }
            if (wvalor_min > wvalor_max)
            {
                e.Cancel = true;
                MessageBox.Show("Valor mínimo maior que valor máximo!", "Atenção!");
            }
        }
        private void valida_campo_nulo(CancelEventArgs e, string txt)
        {
            if (String.IsNullOrEmpty(txt))
            {
                e.Cancel = true;
                MessageBox.Show("Valor não pode ser nulo!", "Atenção!");
            }
        }
        private void txt_ar_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(txt_ar_min.Text) || String.IsNullOrEmpty(txt_ar_max.Text))
            {
                e.Cancel = true;
                MessageBox.Show("Valor não pode ser nulo!", "Atenção!");
            }
            else
            {
                valida_intervalo(e, Constants.ar_min, Constants.ar_max, double.Parse(txt_ar_min.Text), double.Parse(txt_ar_max.Text));
            }
        }

        private void txt_iw_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(txt_iw_min.Text) || String.IsNullOrEmpty(txt_iw_max.Text))
            {
                e.Cancel = true;
                MessageBox.Show("Valor não pode ser nulo!", "Atenção!");
            }
            else
            {
                valida_intervalo(e, Constants.iw_min, Constants.iw_max, double.Parse(txt_iw_min.Text), double.Parse(txt_iw_max.Text));
            }
        }

        private void txt_al_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(txt_al_min.Text) || String.IsNullOrEmpty(txt_al_max.Text))
            {
                e.Cancel = true;
                MessageBox.Show("Valor não pode ser nulo!", "Atenção!");
            }
            else
            {
                valida_intervalo(e, Constants.al_min, Constants.al_max, double.Parse(txt_al_min.Text), double.Parse(txt_al_max.Text));
            }
        }
        private void txt_h_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(txt_h_min.Text) || String.IsNullOrEmpty(txt_h_max.Text))
            {
                e.Cancel = true;
                MessageBox.Show("Valor não pode ser nulo!", "Atenção!");
            }
            else
            {
                valida_intervalo(e, Constants.h_min, Constants.h_max, double.Parse(txt_h_min.Text), double.Parse(txt_h_max.Text));
            }
        }
        private void txt_xcgac_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(txt_xcgac_min.Text) || String.IsNullOrEmpty(txt_xcgac_max.Text))
            {
                e.Cancel = true;
                MessageBox.Show("Valor não pode ser nulo!", "Atenção!");
            }
            else
            {
                valida_intervalo(e, Constants.xcgac_min, Constants.xcgac_max, double.Parse(txt_xcgac_min.Text), double.Parse(txt_xcgac_max.Text));
            }
        }
        private void txt_xw_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(txt_xw_min.Text) || String.IsNullOrEmpty(txt_xw_max.Text))
            {
                e.Cancel = true;
                MessageBox.Show("Valor não pode ser nulo!", "Atenção!");
            }
            else
            {
                valida_intervalo(e, Constants.xw_min, Constants.xw_max, double.Parse(txt_xw_min.Text), double.Parse(txt_xw_max.Text));
            }
        }

        private void txt_cl_asa_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(txt_cl_asa_min.Text) || String.IsNullOrEmpty(txt_cl_asa_max.Text))
            {
                e.Cancel = true;
                MessageBox.Show("Valor não pode ser nulo!", "Atenção!");
            }
            else
            {
                valida_intervalo(e, Constants.cl_asa_min, Constants.cl_asa_max, double.Parse(txt_cl_asa_min.Text), double.Parse(txt_cl_asa_max.Text));
            }
        }
        private void txt_lht_eh_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(txt_lht_eh_min.Text) || String.IsNullOrEmpty(txt_lht_eh_max.Text))
            {
                e.Cancel = true;
                MessageBox.Show("Valor não pode ser nulo!", "Atenção!");
            }
            else
            {

                valida_intervalo(e, Constants.lht_eh_min, Constants.lht_eh_max, double.Parse(txt_lht_eh_min.Text), double.Parse(txt_lht_eh_max.Text));
            }
        }
        private void txt_vlh_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(txt_vlh_min.Text) || String.IsNullOrEmpty(txt_vlh_max.Text))
            {
                e.Cancel = true;
                MessageBox.Show("Valor não pode ser nulo!", "Atenção!");
            }
            else
            {
                valida_intervalo(e, Constants.vlh_min, Constants.vlh_max, double.Parse(txt_vlh_min.Text), double.Parse(txt_vlh_max.Text));
            }
        }
        private void txt_ar_eh_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(txt_ar_eh_min.Text) || String.IsNullOrEmpty(txt_ar_eh_max.Text))
            {
                e.Cancel = true;
                MessageBox.Show("Valor não pode ser nulo!", "Atenção!");
            }
            else
            {
                valida_intervalo(e, Constants.ar_eh_min, Constants.ar_eh_max, double.Parse(txt_ar_eh_min.Text), double.Parse(txt_ar_eh_max.Text));
            }
        }
        private void txt_it_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(txt_it_min.Text) || String.IsNullOrEmpty(txt_it_max.Text))
            {
                e.Cancel = true;
                MessageBox.Show("Valor não pode ser nulo!", "Atenção!");
            }
            else
            {
                valida_intervalo(e, Constants.it_min, Constants.it_max, double.Parse(txt_it_min.Text), double.Parse(txt_it_max.Text));
            }
        }
        
        private void txt_lht_ev_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(txt_lht_ev_min.Text) || String.IsNullOrEmpty(txt_lht_ev_max.Text))
            {
                e.Cancel = true;
                MessageBox.Show("Valor não pode ser nulo!", "Atenção!");
            }
            else
            {
                valida_intervalo(e, Constants.lht_ev_min, Constants.lht_ev_max, double.Parse(txt_lht_ev_min.Text), double.Parse(txt_lht_ev_max.Text));
            }
        }
        
    
      

        private void txt_simulador_Validating(object sender, CancelEventArgs e)
        {
          
        }

      
        private void txt_simulador_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == ' '))
            {
                e.Handled = true;
                MessageBox.Show("Não são permitidos espaços no nome do simulador!", "Atenção!");
            }
        }

        private void btn_export_Click(object sender, EventArgs e)
        {
            double[,] pop = Popini.pop;
            double pontos;
            int i = wmaior_pontuacao;
            Avaliacao avalia = new Avaliacao();
            //pontos = avalia.calculos(pop[1, i], pop[4, i], pop[0, i], pop[2, i], pop[3, i], pop[5, i], pop[9, i], pop[10, i], pop[11, i], pop[12, i], pop[13, i], pop[6, i], pop[7, i], pop[8, i], true);
            pontos = avalia.calculos(double.Parse(txt_iw_result.Text), double.Parse(txt_xcgac_result.Text), double.Parse(txt_ar_result.Text), double.Parse(txt_al_result.Text), double.Parse(txt_h_result.Text), double.Parse(txt_xw_result.Text), double.Parse(txt_lht_eh_result.Text), double.Parse(txt_vlh_result.Text), double.Parse(txt_ar_eh_result.Text), double.Parse(txt_it_result.Text), double.Parse(txt_lht_ev_result.Text), double.Parse(txt_cl_asa_result.Text), double.Parse(txt_tipo_asa_id.Text), double.Parse(txt_tipo_emp_id.Text), true, false);
            //pontos = avalia.calculos(double.Parse(txt_iw_result.Text), double.Parse(txt_xcgac_result.Text), double.Parse(txt_ar_result.Text), double.Parse(txt_al_result.Text), double.Parse(txt_h_result.Text), double.Parse(txt_xw_result.Text), double.Parse(txt_lht_eh_result.Text), double.Parse(txt_vlh_result.Text), double.Parse(txt_ar_eh_result.Text), double.Parse(txt_it_result.Text), double.Parse(txt_lht_ev_result.Text), double.Parse(txt_cl_asa_result.Text), 1, 1, true);
            MessageBox.Show("Exportado com sucesso!");
        }

        private void btn_process_Click(object sender, EventArgs e)
        {
           
            if (String.IsNullOrEmpty(txt_geracoes.Text))
            {
                MessageBox.Show("Necessário preencher o número de gerações.");
                txt_geracoes.Focus();
            }
            else
            {
                if (String.IsNullOrEmpty(txt_simulador.Text))
                {
                    MessageBox.Show("Necessário preencher o nome do simulador.");
                    txt_simulador.Focus();
                }
                else
                {
                    if (!(String.IsNullOrEmpty(txt_ar_min.Text) || String.IsNullOrEmpty(txt_ar_max.Text) || String.IsNullOrEmpty(txt_ar_min.Text) || String.IsNullOrEmpty(txt_iw_max.Text)
                        || String.IsNullOrEmpty(txt_al_min.Text) || String.IsNullOrEmpty(txt_al_max.Text) || String.IsNullOrEmpty(txt_h_min.Text) || String.IsNullOrEmpty(txt_h_max.Text)
                        || String.IsNullOrEmpty(txt_xcgac_min.Text) || String.IsNullOrEmpty(txt_xcgac_max.Text) || String.IsNullOrEmpty(txt_xw_min.Text) || String.IsNullOrEmpty(txt_xw_max.Text)
                        || String.IsNullOrEmpty(txt_cl_asa_min.Text) || String.IsNullOrEmpty(txt_cl_asa_max.Text) || String.IsNullOrEmpty(txt_lht_eh_min.Text) || String.IsNullOrEmpty(txt_lht_eh_max.Text)
                        || String.IsNullOrEmpty(txt_vlh_min.Text) || String.IsNullOrEmpty(txt_vlh_max.Text)
                        || String.IsNullOrEmpty(txt_ar_eh_min.Text) || String.IsNullOrEmpty(txt_ar_eh_max.Text) || String.IsNullOrEmpty(txt_it_min.Text) || String.IsNullOrEmpty(txt_it_max.Text)
                        || String.IsNullOrEmpty(txt_lht_ev_min.Text) || String.IsNullOrEmpty(txt_lht_ev_max.Text)))
                   
                    {
                        double pontos;
                        Cursor.Current = Cursors.WaitCursor;
                        Avaliacao avalia = new Avaliacao();
                        Selecao seleciona = new Selecao();

                        btn_export.Enabled = true;
                        btn_process.Enabled = true;

                        wfilename = string.Empty;
                        pBar1.Visible = true;
                        pbar_label.Text = "Carregando...";
                        pbar_label.Visible = true;
                        start_progress();
                        renderWindowControl1.Visible = false;

                        Popini.main(param, int.Parse(txt_geracoes.Text));
                        wmaior_pontuacao = Popini.seleciona_aviao();
                        mostra_resultado();
                        pontos = avalia.calculos(double.Parse(txt_iw_result.Text), double.Parse(txt_xcgac_result.Text), double.Parse(txt_ar_result.Text), double.Parse(txt_al_result.Text), double.Parse(txt_h_result.Text), double.Parse(txt_xw_result.Text), double.Parse(txt_lht_eh_result.Text), double.Parse(txt_vlh_result.Text), double.Parse(txt_ar_eh_result.Text), double.Parse(txt_it_result.Text), double.Parse(txt_lht_ev_result.Text), double.Parse(txt_cl_asa_result.Text), double.Parse(txt_tipo_asa_id.Text), double.Parse(txt_tipo_emp_id.Text), false, true);

                        PythonScript py = new PythonScript(); 
                        py.ExecuteCommand();
                        MessageBox.Show("Fim do processamento.");

                        wfilename = "../../Python/cr001sm CFD.stl";// Directory.GetCurrentDirectory() + "/"cr001sm CFD.stl; //"../../../teste_vtk/bin/Debug/cr001sm CFD.stl";
                        renderWindowControl1_Load();
                        
                        renderWindowControl1.Visible = true;
                        btn_process.Enabled = false;
                        pBar1.Visible = false;
                        pbar_label.Visible = false;
                    }
                }

            }
        }
        private void Form_simulacao_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        
    }
}
