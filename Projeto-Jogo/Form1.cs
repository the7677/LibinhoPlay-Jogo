using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace Projeto_Jogo
{
    public partial class Form1 : Form
    {
        #region VARIÁVEIS GLOBAIS
        
        string[] imagem = new string[20];
        int rodadaCount = 0, slecMapa,aux;
        int circulo, triangulo, quadrado, retangulo, objSelec = 1, pontosRodada = 0;
        int erradas, certas;
        int cont = 20;
        PictureBox[] mapper;
        

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );
        #endregion

        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 30, 30));
            btn_Restart.Enabled = false;
            btn_TTS.Enabled = false;
        }

        #region TIMER
        private void Timer(object sender, EventArgs e)
        {
            cont--;
            lbl_Timer.Text = cont.ToString() + "s";
            if (cont <= 0)
            {
                tempo.Enabled = false;
                changeGameState(false);
                var voiceLoad = new SpeechSynthesizer();
                voiceLoad.Rate = 4;
                voiceLoad.Speak("O tempo se esgotou, infelizmente você não conseguiu fazer a tempo, deseja tentar novamente??!");
                if (MessageBox.Show("O tempo se esgotou, infelizmente você não conseguiu fazer a tempo, deseja tentar novamente??!", "TEMPO ESGOTADO", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    voiceLoad.Speak("Como você é corajoso, você tem espírito de campeão, vamos nessa!!");
                    jogar();
                    cont = 20;
                }
                else
                {
                    voiceLoad.Speak("Entendo, mas estamos esperando você para a próxima jornada, qualquer coisa é só clicar no botão JOGAR, beleza??");
                }
            }
        }
        #endregion

        #region FORM INICIANDO
        private void Form1_Load(object sender, EventArgs e)
        {
            // Carrega o diretório das imagens
            for (int i = 0; i < 20; i++)
            {
                imagem[i] = "..\\..\\..\\Resources\\imagem" + (i).ToString() + ".png";
            }

            if(MessageBox.Show("Olá, sejá bem vindo ao jogo FIGURAS GEOMÉTRICAS!! Clique no SIM para iniciar", 
                "BOAS-VINDAS!!", 
                MessageBoxButtons.YesNo) == DialogResult.No)
            {
                Close();
            }
            

            changeGameState(false);
            NovaRodada();

            // Seleciona uma imagem aleatória
            // selecImagem();
        }
        #endregion

        #region ATIVAR/DESATIVAR
        private void changeGameState(Boolean state)
        {
            pictureBox3.Visible = state;
            button1.Enabled = state;
            button2.Enabled = state;
            button3.Enabled = state;
            button4.Enabled = state;
            button5.Enabled = state;
            button6.Enabled = state;
        }
        #endregion

        #region DLL
        private SpeechSynthesizer voice;

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr one, int two, int three, int four);
        #endregion

        #region MOVENDO APP
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }
        #endregion

        #region BOTÕES

        #region BOTÃO SAIR 
        private void btnSair_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja sair do jogo?", "SAÍDA", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                Close();
            }
        }
        #endregion

        #region BOTÃO FALANTE
        private void btn_TTS_Click(object sender, EventArgs e)
        {
            var voice = new SpeechSynthesizer();
            voice.Speak("ME AJUDE A ACHAR AS FIGURAS GEOMÉTRICAS NA IMAGEM EMBAIXO DE MIM, VOCÊ CONSEGUE?");
        }
        #endregion

        #region BOTÃO AUTORES
        private void pictureBox7_Click_1(object sender, EventArgs e)
        {
            var voiceLoad = new SpeechSynthesizer();
            voiceLoad.Rate = 4;
            voiceLoad.Speak("Os autores do jogo são: Bruno Garcia Carvalho, Carlos Eduardo Gruninger e Pedro Henrique Brito. Este é o nosso projeto de Visual e Games");
            MessageBox.Show("Autores do jogo:\n\n" +
                "Bruno Garcia Carvalho, RA: 226790\n" +
                "Carlos Eduardo Gruninger, RA: 229229\n" +
                "Pedro Henrique Brito, RA: 226146",
                "SOBRE",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            

        }
        #endregion

        #region BOTÃO JOGAR
        private void pictureBox9_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("DESEJA COMEÇAR A JOGAR??", "PLAY!", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                var voiceLoad = new SpeechSynthesizer();
                voiceLoad.Rate = 4;   
                voiceLoad.Speak("Olá, sejá bem vindo ao jogo FIGURAS GEOMÉTRICAS!! Precisamos que você nos ajude a achar as figuras geométricas que estão nas figuras. Você é capaz disso!");
                tempo.Enabled = true;
                rodadaCount = 0;
                changeGameState(true);
                NovaRodada();
                btn_Jogar.Enabled = false;
                btn_Restart.Enabled = true;
                btn_TTS.Enabled = true;
            }
        }
        private void jogar()
        {
            cont = 20;
            tempo.Enabled = true;
            rodadaCount = 0;
            changeGameState(true);
            NovaRodada();
            lblCertas.Text = "0";
            lblErradas.Text = "0";
            btn_Jogar.Enabled = false;
            btn_Restart.Enabled = true;
            btn_TTS.Enabled = true;
        }
        #endregion

        #region BOTÃO REINICIAR
        private void pbx_Reiniciar_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Você deseja reiniciar o jogo??", "Reiniciar", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                jogar();
            }
        }
        #endregion

        #region CONFIRMAÇÃO BOTÕES

        #region Botao 0
        private void button5_Click(object sender, EventArgs e)
        {
            VerificaFigura(0);
        }
        #endregion

        #region Botao 1
        private void button1_Click(object sender, EventArgs e)
        {
            VerificaFigura(1);
        }
        #endregion

        #region Botao 2
        private void button2_Click(object sender, EventArgs e)
        {
            VerificaFigura(2);
        }
        #endregion

        #region Botao 3
        private void button3_Click(object sender, EventArgs e)
        {
            VerificaFigura(3);
        }
        #endregion

        #region Botao 4
        private void button4_Click(object sender, EventArgs e)
        {
            VerificaFigura(4);
        }
        #endregion

        #region Botao 5
        private void button6_Click(object sender, EventArgs e)
        {
            VerificaFigura(5);
        }
        #endregion

        #region VERIFICAÇÃO DE FIGURA
        private void VerificaFigura(int botao)
        {
            switch (objSelec)
            {
                case 1:
                    pbxEncimaTriangulo.Image = Image.FromFile(Directory.GetCurrentDirectory() + "\\objSelecionado.png");

                    if (botao == circulo)
                    {
                        pbxEncimaCirculo.Image = Image.FromFile(Directory.GetCurrentDirectory() + "\\certo.png");
                        certas += 1;
                        pontosRodada += 1;
                        lbl_correcao.Text = "";
                        lblCertas.Text = certas.ToString();
                    }
                    else
                    {
                        pbxEncimaCirculo.Image = Image.FromFile(Directory.GetCurrentDirectory() + "\\errado.png");
                        erradas += 1;
                        lbl_correcao.Text = "Era" + (circulo!=1?"m ":" ") + circulo.ToString() + " círculo" + (circulo!=1 ? "s!" : "!");

                        lblErradas.Text = erradas.ToString();
                    }
                    break;
                case 2:
                    pbxEncimaQuadrado.Image = Image.FromFile(Directory.GetCurrentDirectory() + "\\objSelecionado.png");

                    if (botao == triangulo)
                    {
                        pbxEncimaTriangulo.Image = Image.FromFile(Directory.GetCurrentDirectory() + "\\certo.png");
                        certas += 1;
                        pontosRodada += 1;
                        lbl_correcao.Text = "";
                        lblCertas.Text = certas.ToString();
                    }
                    else
                    {
                        pbxEncimaTriangulo.Image = Image.FromFile(Directory.GetCurrentDirectory() + "\\errado.png");
                        erradas += 1;
                        lbl_correcao.Text = "Era" + (triangulo != 1 ? "m " : " ") + triangulo.ToString() + " triângulo" + (triangulo != 1 ? "s!" : "!");


                        lblErradas.Text = erradas.ToString();
                    }
                    break;
                case 3:
                    pbxEncimaRetangulo.Image = Image.FromFile(Directory.GetCurrentDirectory() + "\\objSelecionado.png");

                    if (botao == quadrado)
                    {
                        pbxEncimaQuadrado.Image = Image.FromFile(Directory.GetCurrentDirectory() + "\\certo.png");
                        certas += 1;
                        pontosRodada += 1;
                        lbl_correcao.Text = "";
                        lblCertas.Text = certas.ToString();
                    }
                    else
                    {
                        pbxEncimaQuadrado.Image = Image.FromFile(Directory.GetCurrentDirectory() + "\\errado.png");
                        erradas += 1;
                        lbl_correcao.Text = "Era" + (quadrado != 1 ? "m " : " ") + quadrado.ToString() + " quadrado" + (quadrado != 1 ? "s!" : "!");

                        lblErradas.Text = erradas.ToString();
                    }
                    break;
                case 4:
                    if (botao == retangulo)
                    {
                        pbxEncimaRetangulo.Image = Image.FromFile(Directory.GetCurrentDirectory() + "\\certo.png");
                        certas += 1;
                        pontosRodada += 1;
                        lbl_correcao.Text = "";
                        lblCertas.Text = certas.ToString();
                    }
                    else
                    {
                        pbxEncimaRetangulo.Image = Image.FromFile(Directory.GetCurrentDirectory() + "\\errado.png");
                        erradas += 1;
                        lbl_correcao.Text = "Era" + (retangulo != 1 ? "m " : " ") + retangulo.ToString() + " retangulo" + (retangulo != 1 ? "s!" : "!");

                        lblErradas.Text = erradas.ToString();
                    }
                    break;
            }

            objSelec += 1;

            if (objSelec > 4)
            {
                NovaRodada();
            }
        }
        #endregion


        #endregion

        #endregion

        #region ALEATORIEDADE DE IMAGEM
        private void NovaRodada()
        {
            objSelec = 1;
            rodadaCount++;
            
            lbl_TTS.Text = "FIGURA " + rodadaCount.ToString() + "/20";

            if (pontosRodada == 4) cont = 20;
            pontosRodada = 0;

            if (rodadaCount == 20)
            {
                var voiceLoad = new SpeechSynthesizer();
                voiceLoad.Rate = 4;
                rodadaCount = 0;
                changeGameState(false);
                btn_Jogar.Enabled = true;
                btn_Restart.Enabled = false;
                btn_TTS.Enabled = false;
                lblCertas.Text = "0";
                lblErradas.Text = "0";
                tempo.Enabled = false;
                voiceLoad.Speak("Minha nossa, você conseguiu passar pelo desafio!! Eu sabia que você era capaz!!");
                MessageBox.Show("VOCÊ GANHOU!! PARABÉNS!!!!!!!", "GRANDE CAMPEÃO");
                voiceLoad.Speak("Caso queira jogar mais, é só clicar no Botão JOGAR que está logo ali em baixo!!");
                
            }

            pbxEncimaCirculo.Image   = Image.FromFile(Directory.GetCurrentDirectory() + "\\objSelecionado.png");
            pbxEncimaTriangulo.Image = null;
            pbxEncimaQuadrado.Image  = null;
            pbxEncimaRetangulo.Image = null;

            Random aleatorio = new Random();
            aux = aleatorio.Next(1, 100);

            slecMapa = aux % 20;

            pictureBox3.Image = Image.FromFile(Directory.GetCurrentDirectory() + imagem[slecMapa]);

            switch (slecMapa)
            {
                case 0:
                    circulo = 0;
                    triangulo = 0;
                    quadrado = 2;
                    retangulo = 5;
                    break;
                case 1:
                    circulo = 2;
                    triangulo = 1;
                    quadrado = 2;
                    retangulo = 2;
                    break;
                case 2:
                    circulo = 2;
                    triangulo = 1;
                    quadrado = 3;
                    retangulo = 4;
                    break;
                case 3:
                    circulo = 0;
                    triangulo = 2;
                    quadrado = 2;
                    retangulo = 4;
                    break;
                case 4:
                    circulo = 3;
                    triangulo = 5;
                    quadrado = 0;
                    retangulo = 3;
                    break;
                case 5:
                    circulo = 0;
                    triangulo = 0;
                    quadrado = 5;
                    retangulo = 2;
                    break;
                case 6:
                    circulo = 2;
                    triangulo = 3;
                    quadrado = 5;
                    retangulo = 1;
                    break;
                case 7:
                    circulo = 1;
                    triangulo = 2;
                    quadrado = 3;
                    retangulo = 3;
                    break;
                case 8:
                    circulo = 0;
                    triangulo = 0;
                    quadrado = 2;
                    retangulo = 4;
                    break;
                case 9:
                    circulo = 2;
                    triangulo = 1;
                    quadrado = 3;
                    retangulo = 0;
                    break;
                case 10:
                    circulo = 2;
                    triangulo = 2;
                    quadrado = 2;
                    retangulo = 2;
                    break;
                case 11:
                    circulo = 2;
                    triangulo = 2;
                    quadrado = 3;
                    retangulo = 2;
                    break;
                case 12:
                    circulo = 3;
                    triangulo = 1;
                    quadrado = 0;
                    retangulo = 1;
                    break;
                case 13:
                    circulo = 1;
                    triangulo = 1;
                    quadrado = 1;
                    retangulo = 1;
                    break;
                case 14:
                    circulo = 1;
                    triangulo = 2;
                    quadrado = 0;
                    retangulo = 3;
                    break;
                case 15:
                    circulo = 3;
                    triangulo = 1;
                    quadrado = 1;
                    retangulo = 0;
                    break;
                case 16:
                    circulo = 1;
                    triangulo = 1;
                    quadrado = 1;
                    retangulo = 1;
                    break;
                case 17:
                    circulo = 1;
                    triangulo = 1;
                    quadrado = 0;
                    retangulo = 4;
                    break;
                case 18:
                    circulo = 3;
                    triangulo = 1;
                    quadrado = 1;
                    retangulo = 0;
                    break;
                case 19:
                    circulo = 0;
                    triangulo = 0;
                    quadrado = 1;
                    retangulo = 2;
                    break;
                
            }
        }
        #endregion

        #region VOZES IMAGENS MENORES
        private void pictureBox8_Click(object sender, EventArgs e)
        {
            //Circulo

            var voiceCirculo = new SpeechSynthesizer();
            voiceCirculo.Speak("Círculo");
        }

        private void lbl_TTS_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            //Triângulo

            var voiceTriangulo = new SpeechSynthesizer();
            voiceTriangulo.Speak("Triângulo");
        }

        private void btn_Jogar_Enter(object sender, EventArgs e)
        {
            if(MessageBox.Show("DESEJA COMEÇAR A JOGAR??", "COMEÇAR!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                return; 
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            //Quadrado

            var voiceQuadrado= new SpeechSynthesizer();
            voiceQuadrado.Speak("Quadrado");
        }
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            //Retângulo

            var voiceRetangulo = new SpeechSynthesizer();
            voiceRetangulo.Speak("Retângulo");
        }
        #endregion

        #region LIXO
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void pictureBox7_Click(object sender, EventArgs e)
        {

        }
        #endregion
    }
}