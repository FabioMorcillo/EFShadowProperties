using EFShadowProperties.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EFShadowProperties
{
    class Program
    {
        static void Main(string[] args)
        {
            var cliente = new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = "Teste Novo"
            };

            SalvarCliente(cliente);

            var clientesOrdemUltimaModificacao = BuscarClientesComOrdemUltimaModificacaoDescendente();

            Console.WriteLine("- Exibindo clientes com ordem de última modificação descendente");
            ExibirClientes(clientesOrdemUltimaModificacao);

            Console.WriteLine("\n- Exibindo as informações do cliente e a shadow property");
            ExibirClientesComShadowProperties();

            Console.WriteLine("Done.");
        }

        private static void SalvarCliente(Cliente cliente)
        {
            using (var context = new EFShadowPropertiesContext())
            {
                context.Add(cliente);

                context.Entry(cliente)
                    .Property("UltimaModificacao").CurrentValue = DateTime.Now;

                context.SaveChanges();
            }
        }

        private static List<Cliente> BuscarClientesComOrdemUltimaModificacaoDescendente()
        {
            using (var context = new EFShadowPropertiesContext())
            {
                return context.Clientes
                    .OrderByDescending(c => EF.Property<DateTime>(c, "UltimaModificacao"))
                    .ToList();
            }
        }

        private static void ExibirClientes(List<Cliente> clientesOrdemUltimaModificacao)
        {
            foreach(var cliente in clientesOrdemUltimaModificacao)
            {
                Console.WriteLine($"Id: {cliente.Id} Nome: {cliente.Nome}");
            }
        }

        private static void ExibirClientesComShadowProperties()
        {
            foreach (var cliente in BuscarClientesComShadowProperty())
            {
                Console.WriteLine($"Id: {cliente.Item1} Nome: {cliente.Item2} Última Modificação: {cliente.Item3}");
            }
        }

        private static List<Tuple<Guid, string, DateTime>> BuscarClientesComShadowProperty()
        {
            using (var context = new EFShadowPropertiesContext())
            {
                return context.Clientes
                    .Select(c => Tuple.Create(c.Id, c.Nome, EF.Property<DateTime>(c, "UltimaModificacao")))
                    .ToList();
            }
        }
    }
}
