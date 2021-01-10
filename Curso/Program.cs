using System;
using System.Linq;
using System.Collections.Generic;
using CursoEFCore.Data;
using CursoEFCore.Domain;
using CursoEFCore.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CursoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        private static void InserirDados()
        {
            var produto = new Produto
            {
                Descricao = "Produto Teste",
                CodigoBarras = "1234567891231",
                Valor = 10.0,
                TipoProduto = TipoProduto.MercadoriaParaRevenda,
                Ativo = true
            };

            using var db = new ApplicationContext();

            db.Produtos.Add(produto);
            db.Set<Produto>().Add(produto);
            db.Entry(produto).State = EntityState.Added;
            db.Add(produto);

            int registros = db.SaveChanges();

            Console.WriteLine($"Total registro(s) : {registros}");
        }

        private static void InserirDadosEmMassa()
        {
            var produto = new Produto
            {
                Descricao = "Produto teste",
                CodigoBarras = "1234567891231",
                Valor = 10.0,
                TipoProduto = TipoProduto.MercadoriaParaRevenda,
                Ativo = true
            };

            var cliente = new Cliente
            {
                Nome = "Rafael Almeida",
                CEP = "99999000",
                Cidade = "Itabaiana",
                Estado = "SE",
                Telefone = "990000011"
            };

            var listaClientes = new[]
            {
                new Cliente
                {
                    Nome = "Teste 1",
                    CEP = "99999000",
                    Cidade = "Itabaiana",
                    Estado = "SE",
                    Telefone = "999470451"
                },
                new Cliente
                {
                    Nome = "Teste 2",
                    CEP = "72880558",
                    Cidade = "Itabaiana",
                    Estado = "SE",
                    Telefone = "995438787"
                }
            };

            using var db = new ApplicationContext();
            //db.AddRange(produto, cliente);
            db.Clientes.AddRange(listaClientes);
            db.Set<Cliente>().AddRange(listaClientes);

            int registros = db.SaveChanges();
            Console.WriteLine($"Total registro(s): {registros}");
        }

        private static void ConsultarDados()
        {
            using var db = new ApplicationContext();

            /* consultaPorSintaxe = (from c in db.Clientes
                                     where c.Id > 0
                                     select c)
                                     .ToList();*/
            
            var consultaPorMetodo = db.Clientes
                .Where(c => c.Id > 0)
                .OrderBy(c => c.Id)
                .ToList();

            foreach (var cliente in consultaPorMetodo)
            {
                Console.WriteLine($"Consultando cliente : {cliente.Id}");
                //db.Clientes.Find(cliente.Id);
                db.Clientes.FirstOrDefault(c => c.Id == cliente.Id);
            }
        }
    
        private static void CadastrarPedido()
        {
            using var db = new ApplicationContext();

            var cliente = db.Clientes.FirstOrDefault();
            var produto = db.Produtos.FirstOrDefault();

            var pedido = new Pedido
            {
                ClienteId = cliente.Id,
                IniciadoEm = DateTime.Now,
                FinalizadoEm = DateTime.Now,
                Observacao = "Pedido teste",
                Status = StatusPedido.Analise,
                TipoFrete = TipoFrete.SemFrete,
                Itens = new List<PedidoItem>
                {
                    new PedidoItem
                    {
                        ProdutoId = produto.Id,
                        Desconto = 0,
                        Quantidade = 1,
                        Valor = 10
                    }
                }
            };

            db.Pedidos.Add(pedido);
            db.SaveChanges();
        }
    
        private static void ConsultarPedidoCarregamentoAdiantado()
        {
            using var db = new ApplicationContext();
            var pedidos = db.Pedidos.Include(p => p.Itens).ThenInclude(i => i.Produto).ToList();

            Console.WriteLine(pedidos.Count);
        }

        private static void AtualizarDados()
        {
            using var db = new ApplicationContext();

            var cliente = new Cliente {
                Id = 1
            };

            var clienteDesconectado = new 
            {
                Nome = "Cliente Desconectado Passo 3",
                Telefone = "36051526"
            };

            db.Attach(cliente);
            db.Entry(cliente).CurrentValues.SetValues(clienteDesconectado);

            db.SaveChanges();
        }
    
        private static void RemoverRegistro()
        {
            using var db = new ApplicationContext();

            //var cliente = db.Clientes.Find(2);
            var cliente = new Cliente { Id = 3 };

            db.Clientes.Remove(cliente);
            db.Remove(cliente);
            db.Entry(cliente).State = EntityState.Deleted;
            db.SaveChanges();
        }
    }
}
