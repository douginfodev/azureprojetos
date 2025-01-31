using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;

namespace validarcpf
{
    public static class fnValidarCPF
    {
        [FunctionName("fnValidarCPF")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# Request.");

            string numerocpf = req.Query["numerocpf"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            numerocpf = numerocpf ?? data?.numerocpf;

            bool isValid = checarCPF(numerocpf);
            string resultado = isValid ? "válido" : "inválido";

            string responseMessage = string.IsNullOrEmpty(numerocpf)
                ? "Digite o número do cpf ?numerocpf=."
                : $"CPF Nº {numerocpf} é {resultado}.";

            return new OkObjectResult(responseMessage);
        }
    


 public static bool checarCPF(string numeroCpf)
 {

        string cpf = new string(numeroCpf); 
         
        if ((cpf.Length != 11) || (cpf.Distinct().Count() == 1))
            return false;

        int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        
        string tempCpf;
        string digito;
        int soma;
        int resto;

        tempCpf = cpf.Substring(0, 9);
        soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        resto = soma % 11;
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;

        digito = resto.ToString();
        tempCpf = tempCpf + digito;
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;

        digito = digito + resto.ToString();

        return cpf.EndsWith(digito);
   }
  }
 }
