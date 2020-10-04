using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BotMachineLearning.Entidades;
using Newtonsoft.Json;

namespace BotMachineLearning
{
    class Program
    {
        private Dictionary<string, string> dados;

        static void LerJsonCriarDic()
        {
            var file = @"Dados\DadosLearning.json";
            var data = JsonConvert.DeserializeObject<ListDados>(File.ReadAllText(file, Encoding.UTF8));

            
            //dados = data.Itens.SelectMany(i => i.id, (i, chave) => new { chave, valor = i.texto })
            //.ToDictionary(i => i.chave, i => i.valor);

            Console.WriteLine("x");
        }



        static void Main(string[] args)
        {
            LerJsonCriarDic();
            Console.ReadKey();
        }
    }
}
