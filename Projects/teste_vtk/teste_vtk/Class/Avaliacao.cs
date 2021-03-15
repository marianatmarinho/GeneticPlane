using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Globalization;

namespace teste_vtk
{
    class Avaliacao
    {
        static class constants
        {
            public const double mi = 0.04; //coeficiente de atrito com pista
            public const double ro = 1.116; //densidade do ar
            public const double g = 9.81; //gravidade
            public const double cf=0.0055;  //coeficiente de fricção do perfil
            public const double efh=0.95;//eficiencia de cauda. Nada mais é do que a razão entre a presão dinâmica da empenagem horizontal pela asa (q=0.5*ro*v²) (admensional)
            public const double dclh=0.088;//derivada da curva cl x alfa da empenagem horizontal (graus^(-1))
            public const double dclv=0.0691;//derivada da curva cl x alfa da empenagem vertical (graus^(-1))
            public const double xca=0.25;//posição do centro aerodinâmico em porcentagem da corda (porcentagem)
            public const double kn=0.005; //fator 1 de estabilidade direcional obtido de dados históricos (admensional)
            public const double krl=1; //fator 2 de estabilidade direcional obtido de dados históricos admensional)
            public const double zw=0; //distância vertical da asa em 25% da corda até a altura da fuselagem
            public const double vmax = 22;
        }

        public double calculos(double iw, double xcgac, double ar, double al, double h, double xw, double lht, double vlh, double ar_eh, double it, double lvt, double cl_asa, double tp_asa, double tp_emp, bool gera_excel, bool gera_aviao)
        { 
            double vcruise=0; //velocidade de cruzeiro
            double w0=17; //peso maximo de decolagem
            double p_cacone=1; //contador de posição do CA no cone
            double i_eh=1; //contador do posicionador da empenagem horizontal
            double av_aptos=0; //contador para aviões aptos (vão pro banco)
            double av_naoaptos=0; //contador para aviões não aptos
            double cl_asa_stall = 0; //coeficiente de sustentação de stall
            double cl_asa_iw = 0; //coeficiente de sustentação na asa no angulo de incidencia
            double cmac = 0;  // %oeficiente de momento do centro aerodinâmico
            double x = 0; // %espessura maxima do perfil escolhido em porcentagem da sua corda
            double dclw = 0; //%derivada da curva cl x alfa da asa (graus^(-1))
            double tipback_ang = 0; //%angulo tipback (igual ou maior que angulo de stall da asa)
            double p_c = 0; // %posição do avião dentro do cone (sentido longitudinal)
            double s = 0; // %area da asa
            double b = 0; // %envenrgadura maxima dentro do cone (de acordo com altura do efeito solo, e posição do avião dentro do cone)
            double delta_array = 0;
            double bret = 0;//  envergadura da parte retangular da asa (m)
            double cr = 0; //%corda da raiz da asa (m)
            double btrap = 0; // envergadura da parte trapezoidal da asa (m)
            double ct = 0; //%corda da ponta da asa (m)
            double cmediatrap = 0; //;%corda média da parte trapezoidal da asa (m)
            double cmediaret = 0; //%corda média da parte retangular da asa (m)
            double sret = 0; //%area da parte retangular da asa
            double strap = 0;// %area da parte trapezoidal
            double cmedia = 0;// %corda média da asa
            double vcompartimento = 0;// %volume do compartimento
            double afus = 0;// %altura da fuselagem
            double lfus=0;// %larfura fuselagem
            double s_eff = 0;// %area efetiva da asa
            double ws = 0;// %carga alar (usando a area efetiva da asa)
            double clcruise = 0;// %coeficiente de sustentação de cruzeiro
            double hcg = 0;// %Altura do cg até o chão
            double d = 0;// %distancia do cg até o trem de pouso principal
            double a_bf_asa = 0;//%Calculo da altura necessaria para o bordo fuga asa (tipback)
            double ba_ct_x = 0; //posição x do bordo de ataque da asa
            double bf_ct_x = 0; //posição x do bordo de fuga da asa
            double ba_ct_y = 0; //posicão y do bordo de ataque da asa
            double bf_ct_y = 0; //posição y do bordo de fuga da asa
            double ba_ct_z = 0; //altura do bordo de ataque da asa
            double bf_ct_z = 0; //altura do bordo de fuga da asa
            double h_ba_ct = 0; //calculo da hipotenusa formada pelo triangulo das posições x e y do bordo de ataque da asa
            double z_cone_ba = 0; //calculo da altura dispinvel dentro do cone utilizando a equaçao de reta do cone e a hipotenusa do bordo de ataque
            double h_bf_ct = 0; //calculo da hipotenusa formada pelo triangulo das posições x e y do bordo de fuga da asa
            double z_cone_bf = 0; //calculo da altura dispinvel dentro do cone utilizando a equaçao de reta do cone e a hipotenusa do bordo de fuga
            double sht = 0;// %area empenagem horizontal
            double b_eh = 0;// %envergadura da empenagem horizontal (f11(1) de 3)
            double c_eh = 0; //%corda da empenagem horizontal
            double a_eh = 0; // %Calculo a_eh (tipback)
            double ba_c_eh_x = 0; //%posição x do bordo de ataque da empenagem horizontal
            double bf_c_eh_x = 0;// %posição x do bordo de fuga da empenagem horizontal
            double ba_c_eh_y = 0;// %posição y do bordo de ataque da empenagem horizontal
            double bf_c_eh_y = 0; //%posição y do bordo de fuga da empenagem horizontal
            double ba_c_eh_z = 0;// %altura do bordo de ataque da empenagem horizontal
            double bf_c_eh_z = 0; //%altura do bordo de fuga da empenagem horizontal
            double h_ba_ct_eh = 0;// %calculo da hipotenusa formada pelo triangulo das posições x e y do bordo de ataque da empenagem horizontal
            double z_cone_ba_eh = 0; //%calculo da altura dispinvel dentro do cone utilizando a equaçao de reta do cone e a hipotenusa do bordo de ataque
            double h_bf_ct_eh = 0;// %calculo da altura dispinvel dentro do cone utilizando a equaçao de reta do cone e a hipotenusa do bordo de ataque
            double z_cone_bf_eh = 0;// %calculo da altura dispinvel dentro do cone utilizando a equaçao de reta do cone e a hipotenusa do bordo de fuga
            double cl0=0;
            double cm0w = 0;
            double dcmw = 0;
            double cm0f = 0;
            double dcmf = 0;
            double downwash = 0;
            double ddownwash = 0;
            double cm0ht = 0;
            double dcmht = 0;
            double cm0 = 0;
            double dcm = 0;
            double pn = 0;
            double me = 0;
            double sf = 0;  //%area lateral da fuselagem considerando que o tail não esta entelado
            double df = 0;
            double svt_total = 0; //%area vertical total
            double c_ev = 0; //%corda raiz da empenagem vertical 
            double z_cone_ba_ev_r = 0; //altura do cone no bordo de ataque da empenagem vertical do centro
            double z_cone_bf_ev_r = 0; //altura do cone no bordo de fuga da empenagem vertical do centro
            double ba_ev_d_r = 0; //altura disponivel para o bordo de ataque da empenagem vertical do centro
            double bf_ev_d_r = 0; //altura disponivel para o bordo de fuga da empenagem vertical no centro
            double sdvt_r = 0; //area disponivel para a empenagem vertical do centro
            double svt_r = 0; //area utilizada para a empenagem vertical do centro 
            double ba_c_ev_y_inicial = 0;
            double bf_c_ev_y = 0;
            double h_ba_ct_ev_t = 0;
            double z_cone_ba_ev_t = 0;
            double h_bf_ct_ev_t = 0;
            double z_cone_bf_ev_t = 0;
            double ba_ev_d_t = 0;
            double bf_ev_d_t = 0;
            double sdvt_t = 0;
            double bf_ev_r = 0;
            double ba_ev_r = 0;
            double svt_t = 0;
            double bf_ev_t = 0;
            double ba_ev_t = 0;
            double ba_c_ev_y = 0;
            double sdvt_total = 0;
            double dcnwf = 0;
            double dcnv = 0;
            double dcn = 0;
            double n = 0; //calculo arrasto de helice (retirado do programa de polar de arrasto de 2015)
            double beta = 0; //calculo arrasto de helice (retirado do programa de polar de arrasto de 2015)
            double area_helice = 0; //calculo arrasto de helice (retirado do programa de polar de arrasto de 2015)
            double cd_helice = 0; //calculo arrasto de helice (retirado do programa de polar de arrasto de 2015)
            double swfus = 0; //area molhada da fuselagem e tailboom
            double swet = 0; //area molhada do avião
            double cd0 = 0; //arrasto parasita (acrscimo de 10// no arrasto, por causa do arrasto de excrescencia)
            double e0 = 0; //fator de eficiencia de Oswald
            double fi = 0; //efeito solo
            double k = 0; //
            double cll0 = 0; //valor de cl no angulo de incidencia de decolagem
            double cdl0 = 0; //valor de cd no angulo de incidencia de decolagem
            double cdcruise = 0; //cd de cruzeiro
            double clstall = 0; //cl de stall
            double cdstall = 0; //cd de stall
            double vstall = 0; //calculo da velocidade de stall
            double vtakeoff = 0; //calculo da velocidade de decolagem
            double ll0 = 0; //sustentação em situação de decolagem
            double dl0 = 0; //arrasto em situacão de decolagem
            double efaeroto = 0; //eficiencia aerodinamica na decolagem (takeoff)
            double ttakeoff = 0; //tração na decolagem (requerida??)
            double td = 0; //tração disponivel
            double sl0=0; //equação de pouso
            double w = 0; //peso vazio do aviao


            double wp = 0; //calclo de carga paga
            double e_e = 0; //calculo de eficiencia estrutural
            double pcp=0; //pontuação carga paga
            double pee=0; //pontuação eficiencia estrutural
            double pvoo=0; //pontuação total     
                                    

            vcruise = 0.9 *constants.vmax;

           
            cl_asa_iw = 0.0498 * iw + 0.73;
            cl_asa_stall = cl_asa;
            cmac = -0.27;
            x = 0.12;
            dclw = 0.0498;
            tipback_ang = 16;

          //  }

            while (w0>=0)
            {
                while (p_cacone <= 1.25)
                { 
                     p_c=1.25-p_cacone;
                     delta_array=-0.2451*(Math.Pow(al,6.0))- 0.1621*(Math.Pow(al,5)) + 2.1512*Math.Pow(al,4) - 3.4329*Math.Pow(al,3) + 2.4001*Math.Pow(al,2) - 0.7743*al + 0.0971;
                     b = 2 *Math.Sqrt (Math.Pow((( (h + 0.045) - 0.75) / -0.6), 2) -Math.Pow(p_c, 2));
                     s=Math.Pow(b,2)/ar;


                    bret=xw*b/2;
                    btrap=b/2-bret;
                    cr=s/(2*bret+btrap*(1+al));
                    ct=al*cr; 
                    cmediatrap=2/3*cr*(1+al+Math.Pow(al,2)/(1+al));
                    cmediaret=cr;
                    sret=bret*cr*2; 
                    strap=s-sret; 
                    cmedia=(cmediaret*sret+cmediatrap*strap)/s; 
                    vcompartimento=w0/7850+0.001; 
                    afus=h+0.045-0.04; 
                    lfus=vcompartimento/(cr*afus); 
                    s_eff=s-lfus*cr; 
                    ws=w0*9.81/s_eff;
                    clcruise=2*w0*constants.g/(constants.ro*Math.Pow(vcruise,2)*s_eff);
                                
                    hcg=3*(h+0.05)/5; 
                    d=Math.Tan(Math.PI/180*(tipback_ang-iw))*hcg; 
                    a_bf_asa=Math.Tan(Math.PI/180*tipback_ang)*(0.75*cr-d-xcgac*cmedia);
                                
                    if (a_bf_asa>h) //Verifica se a altura necessaria para o bordo de fuga da asa e menor que a altura usada para o efeito solo
                    {    
                         av_naoaptos=av_naoaptos+1;
                         break;        
                    }

                        ba_ct_x=p_c+0.25*ct; 
                        bf_ct_x=p_c-0.75*ct; 
                        ba_ct_y=b/2; 
                        bf_ct_y=b/2;
                        ba_ct_z=h+cmedia*x; 
                        bf_ct_z=h+cmedia*x;
                        h_ba_ct=Math.Sqrt(Math.Pow(ba_ct_x,2)+Math.Pow(ba_ct_y,2));
                        z_cone_ba= -0.6*h_ba_ct +0.75;
                        h_bf_ct=Math.Sqrt(Math.Pow(bf_ct_x,2)+Math.Pow(bf_ct_y,2)); 
                        z_cone_bf= -0.6*h_bf_ct +0.75; 

                        sht=vlh*cmedia*s_eff/lht;
                        b_eh=Math.Sqrt(sht*ar_eh); 
                        c_eh=sht/b_eh;
                        a_eh=Math.Tan(Math.PI/180*(tipback_ang-iw))*(lht+(0.75*c_eh)-d-(xcgac*cmedia)); 

                        ba_c_eh_x=lht-0.25*c_eh-p_c;
                        bf_c_eh_x=lht+0.75*c_eh-p_c;
                        ba_c_eh_y=b_eh/2; 
                        bf_c_eh_y=b_eh/2; 
                        ba_c_eh_z=a_eh+c_eh*0.1;
                        bf_c_eh_z=a_eh+c_eh*0.1;
                        h_ba_ct_eh=Math.Sqrt(Math.Pow(ba_c_eh_x,2)+Math.Pow(ba_c_eh_y,2)); 
                        z_cone_ba_eh= -0.6*h_ba_ct_eh +0.75;
                        h_bf_ct_eh=Math.Sqrt(Math.Pow(bf_c_eh_x,2)+Math.Pow(bf_c_eh_y,2)); 
                        z_cone_bf_eh= -0.6*h_bf_ct_eh +0.75;

                        if (bf_c_eh_z >= z_cone_bf_eh) //%verifica se a altura do bordo de fuga da empenagem horizontal é menor que a altura disponivel do cone, se for menor, o while para e a aeronave é valida
                        {
                            av_naoaptos = av_naoaptos + 1;
                            break;
                        }
                    
                      //calculo do comportamento longitudinal estatico da asa em regime de cruzeiro
                    cl0=dclw*(0-(-10));
                    cm0w=cmac+cl0*xcgac;
                    dcmw=dclw*xcgac;
                    //considera-se que a contribuição da fuselagem para o comportamento longitudinal da aeronave é nulo
                    cm0f=0;
                    dcmf=0;
                    //calculo do comportamento longitudinal estatico da empenagem em regime de cruzeiro, sem deflexão de comando
                    downwash=57.3*2*clcruise/(Math.PI*ar);
                    ddownwash=dclw*57.3/(Math.PI*ar);
                    cm0ht=vlh*constants.efh*constants.dclh*(iw-it+downwash);
                    dcmht=-vlh*constants.efh*constants.dclh*(1-ddownwash);

                   
                    //calculo do comportamento longitudinal da aeronave completa
                    cm0=cm0w+cm0f+cm0ht;
                    dcm=dcmw+dcmf+dcmht;

                    ////margem estatica da aeronave, considerando posição do centro aerodinâmico a 25% da corda, partindo do bordo de ataque
                    pn=constants.xca-dcmf/constants.dclh+vlh*constants.efh*constants.dclh/dclw*(1-ddownwash); //%ponto neutro
                    me=(pn-(xcgac+constants.xca))*100; //%margem estatica

                    //calculo do comportamento da estabilidade direcional estatica da asa+fuselagem
                    sf= cr*afus;  //%area lateral da fuselagem considerando que o tail não esta entelado
                    df=afus;
                    svt_total=vlh*b*s_eff/lvt; //%area vertical total
                    c_ev=c_eh; //%corda raiz da empenagem vertical
                                            
                    //dimensionamento estabilizador vertical raiz
                    z_cone_ba_ev_r=-0.6*(ba_c_eh_x)+0.75; //altura do cone no bordo de ataque da empenagem vertical do centro
                    z_cone_bf_ev_r=-0.6*(bf_c_eh_x)+0.75; //altura do cone no bordo de fuga da empenagem vertical do centro
                    ba_ev_d_r=z_cone_ba_ev_r-ba_c_eh_z; //altura disponivel para o bordo de ataque da empenagem vertical do centro
                    bf_ev_d_r=z_cone_bf_ev_r-bf_c_eh_z; //altura disponivel para o bordo de fuga da empenagem vertical no centro
                    sdvt_r=(ba_ev_d_r+bf_ev_d_r)*c_ev/2; //area disponivel para a empenagem vertical do centro
                    svt_r=sdvt_r; //area utilizada para a empenagem vertical do centro
                                            
                    ba_c_ev_y_inicial=b_eh/2; //contador para a posição da empenagem vertical das pontas
                                            
                    while (ba_c_ev_y_inicial>0)
                    {
                        //dimensionamento estabilizador vertical das pontas
                        bf_c_ev_y=ba_c_ev_y_inicial; //posição y do bordo de fuga da empenagem vertical da ponta = posicão y do bordo de ataque da empenagem vertical da ponta
                        h_ba_ct_ev_t=Math.Sqrt(Math.Pow(ba_c_eh_x,2)+Math.Pow(ba_c_ev_y_inicial,2)); //hipotenusa do bordo de ataque da empenagem vertical da ponta
                        z_cone_ba_ev_t= -0.6*h_ba_ct_ev_t +0.75; //altura do cone no bordo de ataque da empenagem vertical da ponta
                        h_bf_ct_ev_t=Math.Sqrt(Math.Pow(bf_c_eh_x,2)+Math.Pow(bf_c_ev_y,2)); //hipotenusa do bordo de fuga da empenagem vertical da ponta
                        z_cone_bf_ev_t= -0.6*h_bf_ct_ev_t +0.75; //altura do cone no bordo de fuga da empenagem vertical da ponta
                                                
                        ba_ev_d_t=z_cone_ba_ev_t-ba_c_eh_z; //altura disponivel para o bordo de ataque da empenagem vertical da ponta
                        bf_ev_d_t=z_cone_bf_ev_t-bf_c_eh_z; //altura disponivel para o bordo de fuga da empenagem vertical da ponta
                        sdvt_t=(ba_ev_d_t+bf_ev_d_t)*c_ev/2; //area disponivel para a empenagem vertical da ponta (como são duas empenagens na ponta, essa area é multiplicada por 2 na linha 301)
                                                
                        //estailizador vertical raiz
                        bf_ev_r=bf_ev_d_r; //altura do bordo de fuga da empenagem vertical do centro = altura disponivel (maximiza o uso no limite traseiro do cone)
                        ba_ev_r=2*svt_r/c_ev-bf_ev_r; //altura do bordo de ataque da empenagem vertical do centro
                                                
                        //estabilizador vertical ponta
                        svt_t=(svt_total-sdvt_r)/2; //area necessaria para a empenagem vertical da ponta
                        bf_ev_t=bf_ev_d_t; //altura do bordo de fuga da empenagem vertical da ponta = altura disponivel (maximiza o uso no limite traseiro do cone)
                        ba_ev_t=2*svt_t/c_ev-bf_ev_t; //altura do bordo de ataque da empenagem vertical da ponta
                        ba_c_ev_y=ba_c_ev_y_inicial; //posicão da empenagem vertical da ponta
                        ba_c_ev_y_inicial=ba_c_ev_y_inicial-0.01; //contador para a posicão da empenagem vertical da ponta, leva a empenagem da ponta pra uma posição mais perto da empenagem do centro a cada iteração.
                                                
                        sdvt_total=sdvt_r+2*sdvt_t; //area total disponivel para as 3 empenagens verticais
                                                
                        if (svt_r==sdvt_r && svt_t<=sdvt_t && svt_total<=sdvt_total) //verifica se a area da raiz é menor que a area disponivel na raiz, verifica se a area da ponta é menor que a area disponivel para a ponta e verifica se a area total é menor que a area total disponivel (esta meio redundante, apenas a terceira hipotese ja seria suficente para o modo como o programa roda hoje)                    
                            break;
                    }

                     dcnwf=-constants.kn*constants.krl*sf*(lht+0.5*cr)/(s_eff*b); //considerando que o cg esta no centro do compartimento de carga
                     dcnv=vlh*constants.dclv*(0.724+3.06*(svt_total/(2*s_eff))+0.4*constants.zw/df+0.009*ar);
                     dcn=dcnwf+dcnv;

                    //calculo arrasto e desempenho de decolagem
                     //CaLCULO ARRASTO DO SISTEMA MOTO-PROPULSOR
                    n = 2; //calculo arrasto de helice (retirado do programa de polar de arrasto de 2015)
                    beta = 10; //calculo arrasto de helice (retirado do programa de polar de arrasto de 2015)
                    area_helice = 0.0085; //calculo arrasto de helice (retirado do programa de polar de arrasto de 2015)
                    cd_helice = n*Math.Pow(0.1*(Math.Cos(Math.PI*180*beta)),2)*area_helice/s_eff; //calculo arrasto de helice (retirado do programa de polar de arrasto de 2015)
                                            
                    swfus=(2*afus*cr+2*lfus*afus+2*lfus*cr)+(2*lvt*afus+2*lvt*lfus); //area molhada da fuselagem e tailboom
                    swet=(s_eff*2+swfus+sht*2+svt_total*2); //area molhada do avião
                    cd0=1.5*1.1*((constants.cf*swet/s_eff)+cd_helice); //arrasto parasita (acrscimo de 10// no arrasto, por causa do arrasto de excrescencia)
                    e0=Math.Pow((1+delta_array),(-1)); //fator de eficiencia de Oswald
                    fi=Math.Pow((16*h/b),2)/(1+Math.Pow( (16*h/b),2)); //efeito solo
                    k=Math.Pow((Math.PI*e0*ar),(-1)); //
                    cll0=cl_asa_iw/(2*fi); //valor de cl no angulo de incidencia de decolagem
                    cdl0=cd0+fi*k*Math.Pow(cll0,2); //valor de cd no angulo de incidencia de decolagem
                    cdcruise=cd0+k*Math.Pow(clcruise,2); //cd de cruzeiro
                    clstall = cl_asa_stall;//cl de stall
                    cdstall=cd0+fi*k*Math.Pow(clstall,2); //cd de stall
                    vstall=Math.Sqrt(2*w0*constants.g/(constants.ro*s_eff*clstall)); //calculo da velocidade de stall

                    vtakeoff=vstall*1.2; //calculo da velocidade de decolagem

                    //System.Console.WriteLine("vtakeoff: " + vtakeoff.ToString());

                    ll0=0.5*constants.ro*0.7*Math.Pow(vtakeoff,2)*s_eff*cll0; //sustentação em situação de decolagem
                    dl0=0.5*constants.ro*0.7*Math.Pow(vtakeoff,2)*s_eff*cdl0; //arrasto em situacão de decolagem
                    efaeroto=ll0/dl0; //eficiencia aerodinamica na decolagem (takeoff)
                    ttakeoff=constants.g*w0*dl0/ll0; //tração na decolagem (requerida??)
                    td=((-0.010518*Math.Pow( (vtakeoff*0.7),2))+(-0.42866*(vtakeoff*0.7)+37.833)); //tração disponivel

                    sl0=Math.Round(1.44*Math.Pow((constants.g*w0),2)/(constants.g*constants.ro*s_eff*clstall*(td-(dl0+constants.mi*(w0*constants.g-ll0)))), 2); //equação de pouso

                     if (sl0>=60 ||cm0<-0.1 ||dcm>=0  ||me<=10 ||lht<=0 ||clstall>2.6 ||dcn<=0)// ||ba_c_ev_y_inicial<0.05 || sdvt_t<=0 ||sdvt_r<=0) //acrescentar
                     {   av_naoaptos=av_naoaptos+1;
                         break;
                     }
                     else
                     {
                         av_aptos=av_aptos+1;
                     } 
                    
 
                    w= 2.288;// peso vazio do aviao
                                                
                                                
                    wp=w0-w; //calclo de carga paga
                    e_e=wp/w; //calculo de eficiencia estrutural
                    pcp=12.5*wp; //pontuação carga paga
                    pee=5*e_e; //pontuação eficiencia estrutural
                    pvoo=pcp+pee; //pontuação total   
                    p_cacone = p_cacone + 0.01;

                    if(av_aptos>0)
                        break;
                }
                if (sl0>=58 && sl0 <= 59 || av_aptos>0)
                    break;
                p_cacone = 1;
                w0 = w0 - 0.1;
                
            }

            if (gera_aviao)
            {
                string name = "variaveis.dat";
                StringBuilder dat = new StringBuilder();

                dat.Capacity = 0;
                dat.Length = 0;
                dat.AppendLine(tp_asa.ToString(CultureInfo.InvariantCulture));
                dat.AppendLine(tp_emp.ToString(CultureInfo.InvariantCulture));
                dat.AppendLine(cr.ToString(CultureInfo.InvariantCulture));
                dat.AppendLine(b.ToString(CultureInfo.InvariantCulture));
                dat.AppendLine(iw.ToString(CultureInfo.InvariantCulture));

                dat.AppendLine(ct.ToString(CultureInfo.InvariantCulture));
                dat.AppendLine(b_eh.ToString(CultureInfo.InvariantCulture));
                dat.AppendLine(c_eh.ToString(CultureInfo.InvariantCulture));
                dat.AppendLine(c_eh.ToString(CultureInfo.InvariantCulture));
                dat.AppendLine(i_eh.ToString(CultureInfo.InvariantCulture));

                dat.AppendLine(lht.ToString(CultureInfo.InvariantCulture));
                dat.AppendLine((bf_c_eh_z-h).ToString(CultureInfo.InvariantCulture));
                dat.AppendLine(c_ev.ToString(CultureInfo.InvariantCulture));
                dat.AppendLine(c_ev.ToString(CultureInfo.InvariantCulture));
                dat.AppendLine(ba_ev_d_r.ToString(CultureInfo.InvariantCulture));

                dat.AppendLine(lfus.ToString(CultureInfo.InvariantCulture));
                dat.AppendLine(afus.ToString(CultureInfo.InvariantCulture));
                dat.AppendLine(xw.ToString(CultureInfo.InvariantCulture));

                // Abrir o arquivo

                StreamWriter arquivo = new StreamWriter("../../Python/" + name, false, Encoding.ASCII);

                // Escreve no arquiv

                arquivo.WriteLine(dat.ToString());

                // Fechar arquivo

                arquivo.Close();

            }

            if (gera_excel==true)
            {
                string asa="Asa";
                string emp = "Emp";
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog1.Title = "Save an Image File";
                saveFileDialog1.ShowDialog();

                if (saveFileDialog1.FileName != "")
                {
                    char wsep = '#';
                    StringBuilder csvcontent = new StringBuilder();

                    csvcontent.Capacity = 0;
                    csvcontent.Length = 0;
                    if (double.Parse(tp_asa.ToString()) == 0)
                    {
                        asa = "Trapezoidal";
                    }
                    else
                    {
                        if (double.Parse(tp_asa.ToString()) == 1)
                            asa = "Retangular";
                        else
                            if (double.Parse(tp_asa.ToString()) == 2)
                                asa = "Mista";
                           
                    }
                    if (double.Parse(tp_emp.ToString()) == 1)
                    {
                        emp = "Convencional";
                    }
                    else
                    {
                        if (double.Parse(tp_emp.ToString()) == 2)
                            emp = "em H";
                        else
                            if (double.Parse(tp_emp.ToString()) == 3)
                                emp = "em U";
                            else
                                if (double.Parse(tp_emp.ToString()) == 4)
                                    emp = "em V";
                    }
                    csvcontent.AppendLine("Tipo Asa" + wsep + asa + wsep);
                    csvcontent.AppendLine("Alongamento Asa" + wsep + ar.ToString() + wsep);
                    csvcontent.AppendLine("Incidencia Asa" + wsep + iw.ToString() + wsep);
                    csvcontent.AppendLine("Afilamento Asa" + wsep + al.ToString() + wsep);
                    csvcontent.AppendLine("Altura Asa" + wsep + h.ToString() + wsep);
                    csvcontent.AppendLine("xcgac" + wsep + xcgac.ToString() + wsep);
                    csvcontent.AppendLine("Variacao Envegadura Asa" + wsep + xw.ToString() + wsep);
                    csvcontent.AppendLine("Coef Sustentacao Asa" + wsep + cl_asa.ToString() + wsep);
                    csvcontent.AppendLine("Tipo Empenagem" + wsep + emp + wsep);
                    csvcontent.AppendLine("Braco EH" + wsep + lht.ToString() + wsep);
                    csvcontent.AppendLine("Volume EH" + wsep + vlh.ToString() + wsep);
                    csvcontent.AppendLine("Alongamento EH" + wsep + ar_eh.ToString() + wsep);
                    csvcontent.AppendLine("Angulo Incidencia EH" + wsep + it.ToString() + wsep);
                    csvcontent.AppendLine("Braco EV" + wsep + lvt.ToString() + wsep);
                    csvcontent.AppendLine("peso vazio do aviao" + wsep + w.ToString() + wsep);
                    csvcontent.AppendLine("calculo de carga paga" + wsep + wp.ToString() + wsep);
                    csvcontent.AppendLine("calculo de eficiencia estrutural" + wsep + e_e.ToString() + wsep);
                    csvcontent.AppendLine("pontuacao carga paga" + wsep + pcp.ToString() + wsep);
                    csvcontent.AppendLine("pontuacao eficiencia estrutural" + wsep + pee.ToString() + wsep);
                    csvcontent.AppendLine("pontuacao total" + wsep + pvoo.ToString() + wsep);

                    csvcontent.AppendLine("Velocidade Cruzeiro" + wsep + vcruise.ToString() + wsep);
                    csvcontent.AppendLine("Peso Max Decolagem" + wsep + w0.ToString() + wsep);
                    csvcontent.AppendLine("Coef Sutentacao Asa Stall" + wsep + cl_asa_stall.ToString() + wsep);
                    csvcontent.AppendLine("Coef Sustentacao na Asa no Angulo de Incidencia" + wsep + cl_asa_iw.ToString() + wsep);
                    csvcontent.AppendLine("%Coef Momento CA" + wsep + cmac.ToString() + wsep);
                    csvcontent.AppendLine("%Espessura Max Perfil" + wsep + x.ToString() + wsep);
                    csvcontent.AppendLine("%Derivada da curva cl x alfa asa" + wsep + dclw.ToString() + wsep);
                    csvcontent.AppendLine("%Angulo tipback" + wsep + tipback_ang.ToString() + wsep);
                    csvcontent.AppendLine("%Coef Momento CA" + wsep + cmac.ToString() + wsep);
                    csvcontent.AppendLine("%Posicao Aviao no Cone" + wsep + p_c.ToString() + wsep);
                    csvcontent.AppendLine("%area Asa" + wsep + s.ToString() + wsep);
                    csvcontent.AppendLine("%Envergadura Max Dentro do Cone" + wsep + b.ToString() + wsep);
                    csvcontent.AppendLine("delta_array" + wsep + delta_array.ToString() + wsep);
                    csvcontent.AppendLine("Envergadura da parte retangular da asa" + wsep + bret.ToString() + wsep);
                    csvcontent.AppendLine("%corda da raiz da asa" + wsep + cr.ToString() + wsep);
                    csvcontent.AppendLine("envergadura da parte trapezoidal da asa" + wsep + btrap.ToString() + wsep);
                    csvcontent.AppendLine("%corda da ponta da asa" + wsep + ct.ToString() + wsep);
                    csvcontent.AppendLine("%corda média da parte trapezoidal da asa" + wsep + cmediatrap.ToString() + wsep);
                    csvcontent.AppendLine("%corda média da parte retangular da asa" + wsep + cmediaret.ToString() + wsep);
                    csvcontent.AppendLine("%area da parte retangular da asa" + wsep + sret.ToString() + wsep);
                    csvcontent.AppendLine("%area da parte trapezoidal" + wsep + strap.ToString() + wsep);
                    csvcontent.AppendLine("%corda média da asa" + wsep + cmedia.ToString() + wsep);
                    csvcontent.AppendLine("%volume do compartimento" + wsep + vcompartimento.ToString() + wsep);
                    csvcontent.AppendLine("%altura da fuselagem" + wsep + afus.ToString() + wsep);
                    csvcontent.AppendLine("%larfura fuselagem" + wsep + lfus.ToString() + wsep);
                    csvcontent.AppendLine("%area efetiva da asa" + wsep + s_eff.ToString() + wsep);
                    csvcontent.AppendLine("%carga alar (usando a area efetiva da asa)" + wsep + ws.ToString() + wsep);
                    csvcontent.AppendLine("%coeficiente de sustentacao de cruzeiro" + wsep + clcruise.ToString() + wsep);
                    csvcontent.AppendLine("%Altura do cg até o chao" + wsep + hcg.ToString() + wsep);
                    csvcontent.AppendLine("%distancia do cg até o trem de pouso principal" + wsep + d.ToString() + wsep);
                    csvcontent.AppendLine("%Calculo da altura necessaria para o bordo fuga asa (tipback)" + wsep + a_bf_asa.ToString() + wsep);
                    csvcontent.AppendLine("posicao x do bordo de ataque da asa" + wsep + ba_ct_x.ToString() + wsep);
                    csvcontent.AppendLine("posicao x do bordo de fuga da asa" + wsep + bf_ct_x.ToString() + wsep);
                    csvcontent.AppendLine("posicao y do bordo de ataque da asa" + wsep + ba_ct_y.ToString() + wsep);
                    csvcontent.AppendLine("posicao y do bordo de fuga da asa" + wsep + bf_ct_y.ToString() + wsep);
                    csvcontent.AppendLine("altura do bordo de ataque da asa" + wsep + ba_ct_z.ToString() + wsep);
                    csvcontent.AppendLine("altura do bordo de fuga da asa" + wsep + bf_ct_z.ToString() + wsep);
                    csvcontent.AppendLine("calculo da hipotenusa formada pelo triangulo das posicões x e y do bordo de ataque da asa" + wsep + h_ba_ct.ToString() + wsep);
                    csvcontent.AppendLine("calculo da altura dispinvel dentro do cone utilizando a equacao de reta do cone e a hipotenusa do bordo de ataque" + wsep + z_cone_ba.ToString() + wsep);
                    csvcontent.AppendLine("calculo da hipotenusa formada pelo triangulo das posicões x e y do bordo de fuga da asa" + wsep + h_bf_ct.ToString() + wsep);
                    csvcontent.AppendLine("/calculo da altura dispinvel dentro do cone utilizando a equacao de reta do cone e a hipotenusa do bordo de fuga" + wsep + z_cone_bf.ToString() + wsep);
                    csvcontent.AppendLine("%area empenagem horizontal" + wsep + sht.ToString() + wsep);
                    csvcontent.AppendLine("%envergadura da empenagem horizontal" + wsep + b_eh.ToString() + wsep);
                    csvcontent.AppendLine("%corda da empenagem horizontal" + wsep + c_eh.ToString() + wsep);
                    csvcontent.AppendLine("%Calculo a_eh (tipback)" + wsep + a_eh.ToString() + wsep);
                    csvcontent.AppendLine("%posicao x do bordo de ataque da empenagem horizontal" + wsep + ba_c_eh_x.ToString() + wsep);
                    csvcontent.AppendLine("%posicao x do bordo de fuga da empenagem horizontal" + wsep + bf_c_eh_x.ToString() + wsep);
                    csvcontent.AppendLine("%posicao y do bordo de ataque da empenagem horizontal" + wsep + ba_c_eh_y.ToString() + wsep);
                    csvcontent.AppendLine("%posicao y do bordo de fuga da empenagem horizontal" + wsep + bf_c_eh_y.ToString() + wsep);
                    csvcontent.AppendLine("%altura do bordo de ataque da empenagem horizontal" + wsep + ba_c_eh_z.ToString() + wsep);
                    csvcontent.AppendLine("%altura do bordo de fuga da empenagem horizontal" + wsep + bf_c_eh_z.ToString() + wsep);
                    csvcontent.AppendLine(" %calculo da hipotenusa formada pelo triangulo das posições x e y do bordo de ataque da empenagem horizontal" + wsep + h_ba_ct_eh.ToString() + wsep);
                    csvcontent.AppendLine("%calculo da altura dispinvel dentro do cone utilizando a equacao de reta do cone e a hipotenusa do bordo de ataque" + wsep + z_cone_ba_eh.ToString() + wsep);
                    csvcontent.AppendLine("%calculo da altura dispinvel dentro do cone utilizando a equacao de reta do cone e a hipotenusa do bordo de ataque" + wsep + h_bf_ct_eh.ToString() + wsep);
                    csvcontent.AppendLine("%calculo da altura dispinvel dentro do cone utilizando a equacao de reta do cone e a hipotenusa do bordo de fuga" + wsep + z_cone_bf_eh.ToString() + wsep);
                    csvcontent.AppendLine("cl0" + wsep + cl0.ToString() + wsep);
                    csvcontent.AppendLine("cm0w" + wsep + cm0w.ToString() + wsep);
                    csvcontent.AppendLine("dcmw" + wsep + dcmw.ToString() + wsep);
                    csvcontent.AppendLine("cm0f" + wsep + cm0f.ToString() + wsep);
                    csvcontent.AppendLine("dcmf" + wsep + dcmf.ToString() + wsep);
                    csvcontent.AppendLine("downwash" + wsep + downwash.ToString() + wsep);
                    csvcontent.AppendLine("ddownwash" + wsep + ddownwash.ToString() + wsep);
                    csvcontent.AppendLine("cm0ht" + wsep + cm0ht.ToString() + wsep);
                    csvcontent.AppendLine("dcmht" + wsep + dcmht.ToString() + wsep);
                    csvcontent.AppendLine("cm0" + wsep + cm0.ToString() + wsep);
                    csvcontent.AppendLine("dcm" + wsep + dcm.ToString() + wsep);
                    csvcontent.AppendLine("pn" + wsep + pn.ToString() + wsep);
                    csvcontent.AppendLine("me" + wsep + me.ToString() + wsep);
                    csvcontent.AppendLine("%area lateral da fuselagem considerando que o tail nao esta entelado" + wsep + sf.ToString() + wsep);
                    csvcontent.AppendLine("df" + wsep + df.ToString() + wsep);
                    csvcontent.AppendLine("%area vertical total" + wsep + svt_total.ToString() + wsep);
                    csvcontent.AppendLine("%corda raiz da empenagem vertical " + wsep + c_ev.ToString() + wsep);
                    csvcontent.AppendLine("altura do cone no bordo de ataque da empenagem vertical do centro" + wsep + z_cone_ba_ev_r.ToString() + wsep);
                    csvcontent.AppendLine("altura do cone no bordo de fuga da empenagem vertical do centro" + wsep + z_cone_bf_ev_r.ToString() + wsep);
                    csvcontent.AppendLine("altura disponivel para o bordo de ataque da empenagem vertical do centro" + wsep + ba_ev_d_r.ToString() + wsep);
                    csvcontent.AppendLine("altura disponivel para o bordo de fuga da empenagem vertical no centro" + wsep + bf_ev_d_r.ToString() + wsep);
                    csvcontent.AppendLine("area disponivel para a empenagem vertical do centro" + wsep + sdvt_r.ToString() + wsep);
                    csvcontent.AppendLine("area utilizada para a empenagem vertical do centro " + wsep + svt_r.ToString() + wsep);
                    csvcontent.AppendLine("ba_c_ev_y_inicial" + wsep + ba_c_ev_y_inicial.ToString() + wsep);
                    csvcontent.AppendLine("bf_c_ev_y" + wsep + bf_c_ev_y.ToString() + wsep);
                    csvcontent.AppendLine("h_ba_ct_ev_t" + wsep + h_ba_ct_ev_t.ToString() + wsep);
                    csvcontent.AppendLine("z_cone_ba_ev_t" + wsep + z_cone_ba_ev_t.ToString() + wsep);
                    csvcontent.AppendLine("h_bf_ct_ev_t" + wsep + h_bf_ct_ev_t.ToString() + wsep);
                    csvcontent.AppendLine("z_cone_bf_ev_t" + wsep + z_cone_bf_ev_t.ToString() + wsep);
                    csvcontent.AppendLine("ba_ev_d_t" + wsep + ba_ev_d_t.ToString() + wsep);
                    csvcontent.AppendLine("bf_ev_d_t" + wsep + bf_ev_d_t.ToString() + wsep);
                    csvcontent.AppendLine("sdvt_t" + wsep + sdvt_t.ToString() + wsep);
                    csvcontent.AppendLine("bf_ev_r" + wsep + bf_ev_r.ToString() + wsep);
                    csvcontent.AppendLine("ba_ev_r" + wsep + ba_ev_r.ToString() + wsep);
                    csvcontent.AppendLine("svt_t" + wsep + svt_t.ToString() + wsep);
                    csvcontent.AppendLine("bf_ev_t" + wsep + bf_ev_t.ToString() + wsep);
                    csvcontent.AppendLine("ba_ev_t" + wsep + ba_ev_t.ToString() + wsep);
                    csvcontent.AppendLine("ba_c_ev_y" + wsep + ba_c_ev_y.ToString() + wsep);
                    csvcontent.AppendLine("sdvt_total" + wsep + sdvt_total.ToString() + wsep);
                    csvcontent.AppendLine("dcnwf" + wsep + dcnwf.ToString() + wsep);
                    csvcontent.AppendLine("dcnv" + wsep + dcnv.ToString() + wsep);
                    csvcontent.AppendLine("dcn" + wsep + dcn.ToString() + wsep);
                    csvcontent.AppendLine("area molhada da fuselagem e tailboom" + wsep + swfus.ToString() + wsep);
                    csvcontent.AppendLine("area molhada do aviao" + wsep + swet.ToString() + wsep);
                    csvcontent.AppendLine("arrasto parasita " + wsep + cd0.ToString() + wsep);
                    csvcontent.AppendLine("fator de eficiencia de Oswald" + wsep + e0.ToString() + wsep);
                    csvcontent.AppendLine("efeito solo" + wsep + fi.ToString() + wsep);
                    csvcontent.AppendLine("k" + wsep + k.ToString() + wsep);
                    csvcontent.AppendLine("valor de cl no angulo de incidencia de decolagem" + wsep + cll0.ToString() + wsep);
                    csvcontent.AppendLine("valor de cd no angulo de incidencia de decolagem" + wsep + cdl0.ToString() + wsep);
                    csvcontent.AppendLine("cd de cruzeiro" + wsep + cdcruise.ToString() + wsep);
                    csvcontent.AppendLine("cl de stall" + wsep + clstall.ToString() + wsep);
                    csvcontent.AppendLine("cd de stall" + wsep + cdstall.ToString() + wsep);
                    csvcontent.AppendLine("calculo da velocidade de stall" + wsep + vstall.ToString() + wsep);
                    csvcontent.AppendLine("calculo da velocidade de decolagem" + wsep + vtakeoff.ToString() + wsep);
                    csvcontent.AppendLine("sustentacao em situacao de decolagem" + wsep + ll0.ToString() + wsep);
                    csvcontent.AppendLine("arrasto em situacao de decolagem" + wsep + dl0.ToString() + wsep);
                    csvcontent.AppendLine("eficiencia aerodinamica na decolagem (takeoff)" + wsep + efaeroto.ToString() + wsep);
                    csvcontent.AppendLine("tracao na decolagem " + wsep + ttakeoff.ToString() + wsep);
                    csvcontent.AppendLine("tracao disponivel" + wsep + td.ToString() + wsep);
                    csvcontent.AppendLine("equacao de pouso" + wsep + sl0.ToString() + wsep);

           
                    
                    //File.AppendAllText(saveFileDialog1.FileName, csvcontent.ToString());
                    // Abrir o arquivo

                    StreamWriter arquivo = new StreamWriter(saveFileDialog1.FileName, false, Encoding.ASCII);

                    // Escreve no arquivo

                    arquivo.WriteLine(csvcontent.ToString());

                    // Fechar arquivo

                    arquivo.Close();
                    
                }
            }
            return pvoo;
        }

    }
}
