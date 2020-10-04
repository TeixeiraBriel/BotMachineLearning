using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using BotMachineLearning.Entidades;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static PollingLibrary.Polling;

namespace BotMachineLearning
{
    class Program
    {
        static Dictionary<string, string[]> dados;
        static List<Dados> _Itens = new List<Dados>();

        static void LerJsonCriarDic()
        {
            _Itens = new List<Dados>();
            var file = @"Dados\DadosLearningSimples.json";
            var data = JsonConvert.DeserializeObject<ListDados>(File.ReadAllText(file, Encoding.UTF8));

            foreach (var dado in data.Itens)
            {
                _Itens.Add(dado);
            }

            dados = data.Itens.SelectMany(i => i.Pergunta, (i, chave) => new { chave, valor = i.Resposta }).ToDictionary(i => i.chave, i => i.valor);

            //dados = data.Itens.SelectMany(i => i.id, (i, chave) => new { chave, valor = i.texto })
            //.ToDictionary(i => i.chave, i => i.valor);

        }

        static void CriarDic(string[] Pergunta)
        {
            ListDados novo = new ListDados();
            novo.Itens = new List<Dados>();

            Dados NovaInteracao = new Dados();
            NovaInteracao.Pergunta = Pergunta;
            NovaInteracao.Resposta = new string[] { "" };

            var file = @"Dados\DadosLearningSimples.json";
            var data = JsonConvert.DeserializeObject<ListDados>(File.ReadAllText(file, Encoding.UTF8));

            foreach (var dado in data.Itens)
            {
                novo.Itens.Add(dado);
            }

            novo.Itens.Add(NovaInteracao);

            string output = JsonConvert.SerializeObject(novo);

            File.WriteAllText(@"Dados\DadosLearningSimples.json", output.ToString());

            using (StreamWriter file2 = File.CreateText(@"Dados\DadosLearningSimples.json"))
            using (JsonTextWriter writer = new JsonTextWriter(file2))
            {
                JObject.Parse(output).WriteTo(writer);
            }
        }


        static void EditDic(string[] Pergunta, string[] Resposta)
        {
            ListDados novo = new ListDados();
            novo.Itens = new List<Dados>();
            bool existeResposta = false;

            var file = @"Dados\DadosLearningSimples.json";
            var data = JsonConvert.DeserializeObject<ListDados>(File.ReadAllText(file, Encoding.UTF8));

            foreach (var dado in data.Itens)
            {
                if (dado.Pergunta[0] == Pergunta[0])
                {
                    string[] novoContainerResp = new string[dado.Resposta.Length + 1];
                    int i = 0;

                    foreach (var opcao in dado.Resposta)
                    {
                        novoContainerResp[i] = opcao;
                        if (opcao == Resposta[0])
                        {
                            existeResposta = true;
                        }
                        i++;
                    }

                    if (!existeResposta && dado.Resposta[0] != "")
                    {
                        novoContainerResp[i] = Resposta[0];
                        dado.Resposta = novoContainerResp;
                    }
                    else if (dado.Resposta[0] == "")
                    {
                        dado.Resposta = Resposta;
                    }

                    novo.Itens.Add(dado);
                }
                else
                {
                    novo.Itens.Add(dado);
                }
            }

            string output = JsonConvert.SerializeObject(novo);

            File.WriteAllText(@"Dados\DadosLearningSimples.json", output.ToString());

            using (StreamWriter file2 = File.CreateText(@"Dados\DadosLearningSimples.json"))
            using (JsonTextWriter writer = new JsonTextWriter(file2))
            {
                JObject.Parse(output).WriteTo(writer);
            }
        }

        static void fluxoComum(string pergunta)
        {
            if (pergunta == "")
            {
                return;
            }
            if (dados.ContainsKey(pergunta))
            {
                var x = PollSafe(() => dados[pergunta]);
                if (x[0] == "")
                {
                    Console.WriteLine("Bot: Desculpa, nao sei responder.");
                    //EditDic(new string[] { pergunta }, new string[] { "Desculpa, nao sei responder." });
                }
                else
                {
                    Random rand = new Random();

                    Console.WriteLine("Bot: " + x[rand.Next(x.Length)]);
                    Console.WriteLine("");
                }
            }
            else
            {
                CriarDic(new string[] { pergunta });
                Console.WriteLine("Bot: Desculpa, nao sei responder.");
            }
        }

        static void Main(string[] args)
        {
            LerJsonCriarDic();
            string pergunta = "";
            string responderBot = "";
            bool cortouFluxo = false;
            while (true)
            {
                if (!responderBot.Contains("?"))
                {
                    Random rand = new Random();
                    string perguntaBot = _Itens[rand.Next(_Itens.Count)].Pergunta[0];
                    Console.WriteLine("Bot: " + perguntaBot);
                    responderBot = Console.ReadLine();
                    cortouFluxo = true;
                    if (!responderBot.Contains("?") && responderBot != "")
                    {
                        EditDic(new string[] { perguntaBot }, new string[] { responderBot });
                    }
                    else
                    {
                        fluxoComum(responderBot);
                        responderBot = "";
                    }
                }
                else
                {
                    if (!cortouFluxo)
                    {
                        pergunta = Console.ReadLine();
                    }
                    fluxoComum(pergunta);
                    responderBot = "";
                    cortouFluxo = false;
                }
                LerJsonCriarDic();
            }
        }
    }
}
