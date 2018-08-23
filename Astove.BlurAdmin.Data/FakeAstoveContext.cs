using AInBox.Astove.Core.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using System.Threading;
using AInBox.Astove.Core.UnitTest;
using Astove.BlurAdmin.Model.Domain;

namespace Astove.BlurAdmin.Data
{
    public class FakeAstoveContext : IAstoveContext
    {
        public FakeAstoveContext()
        {
            Reset();
        }

        public void Reset()
        {
            var estados = new List<Estado>
            {
                new Estado { Id  = 1, Nome = "Acre", Sigla = "AC", Capital = "Rio Branco", Regiao = "Norte", FaixaDDD = "68" },
                new Estado { Id  = 2, Nome = "Alagoas", Sigla = "AL", Capital = "Maceió", Regiao = "Nordeste", FaixaDDD = "82" },
                new Estado { Id  = 3, Nome = "Amapá", Sigla = "AP", Capital = "Macapá", Regiao = "Norte", FaixaDDD = "96" },
                new Estado { Id  = 4, Nome = "Amazonas", Sigla = "AM", Capital = "Manaus", Regiao = "Norte", FaixaDDD = "97" },
                new Estado { Id  = 5, Nome = "Bahia", Sigla = "BA", Capital = "Salvador", Regiao = "Nordeste", FaixaDDD = "71,73,74,75,77" },
                new Estado { Id  = 6, Nome = "Ceará", Sigla = "CE", Capital = "Fortaleza", Regiao = "Nordeste", FaixaDDD = "85,88" },
                new Estado { Id  = 7, Nome = "Distrito Federal", Sigla = "DF", Capital = "Brasília", Regiao = "Centro Oeste", FaixaDDD = "61" },
                new Estado { Id  = 8, Nome = "Espírito Santo", Sigla = "ES", Capital = "Vitória", Regiao = "Sudeste", FaixaDDD = "27,28" },
                new Estado { Id  = 9, Nome = "Goiás", Sigla = "GO", Capital = "Goiânia", Regiao = "Centro Oeste", FaixaDDD = "61,62,64" },
                new Estado { Id  = 10, Nome = "Maranhão", Sigla = "MA", Capital = "São Luís", Regiao = "Nordeste", FaixaDDD = "98,99" },
                new Estado { Id  = 11, Nome = "Mato Grosso", Sigla = "MT", Capital = "Cuiabá", Regiao = "Centro Oeste", FaixaDDD = "65,66" },
                new Estado { Id  = 12, Nome = "Mato Grosso do Sul", Sigla = "MS", Capital = "Campo Grande", Regiao = "Centro Oeste", FaixaDDD = "67" },
                new Estado { Id  = 13, Nome = "Minas Gerais", Sigla = "MG", Capital = "Belo Horizonte", Regiao = "Sudeste", FaixaDDD = "31,32,33,34,35,37,38" },
                new Estado { Id  = 14, Nome = "Pará", Sigla = "PA", Capital = "Belém", Regiao = "Norte", FaixaDDD = "91,93,94" },
                new Estado { Id  = 15, Nome = "Paraíba", Sigla = "PB", Capital = "João Pessoa", Regiao = "Nordeste", FaixaDDD = "83" },
                new Estado { Id  = 16, Nome = "Paraná", Sigla = "PR", Capital = "Curitiba", Regiao = "Sul", FaixaDDD = "41,42,43,44,45,46" },
                new Estado { Id  = 17, Nome = "Pernambuco", Sigla = "PE", Capital = "Recife", Regiao = "Nordeste", FaixaDDD = "81,87" },
                new Estado { Id  = 18, Nome = "Piauí", Sigla = "PI", Capital = "Teresina", Regiao = "Nordeste", FaixaDDD = "86,89" },
                new Estado { Id  = 19, Nome = "Rio de Janeiro", Sigla = "RJ", Capital = "Rio de Janeiro", Regiao = "Sudeste", FaixaDDD = "21,22,24" },
                new Estado { Id  = 20, Nome = "Rio Grande do Norte", Sigla = "RN", Capital = "Natal", Regiao = "Nordeste", FaixaDDD = "84" },
                new Estado { Id  = 21, Nome = "Rio Grande do Sul", Sigla = "RS", Capital = "Porto Alegre", Regiao = "Sul", FaixaDDD = "51,53,54,55" },
                new Estado { Id  = 22, Nome = "Rondônia", Sigla = "RO", Capital = "Porto Velho", Regiao = "Norte", FaixaDDD = "69" },
                new Estado { Id  = 23, Nome = "Roraima", Sigla = "RR", Capital = "Boa Vista", Regiao = "Norte", FaixaDDD = "95" },
                new Estado { Id  = 24, Nome = "Santa Catarina", Sigla = "SC", Capital = "Florianópolis", Regiao = "Sul", FaixaDDD = "47,48,49" },
                new Estado { Id  = 25, Nome = "São Paulo", Sigla = "SP", Capital = "São Paulo", Regiao = "Sudeste", FaixaDDD = "11,12,13,14,15,16,17,18,19" },
                new Estado { Id  = 26, Nome = "Sergipe", Sigla = "SE", Capital = "Aracaju", Regiao = "Nordeste", FaixaDDD = "79" },
                new Estado { Id  = 27, Nome = "Tocantins", Sigla = "TO", Capital = "Palmas", Regiao = "Norte", FaixaDDD = "63" }
            }.AsQueryable();
            this.Estados = new FakeDbSet<Estado>(new TestDbAsyncEnumerable<Estado>(estados));

            //SELECT concat('new Cidade { Id = ', id, ', EstadoId = ', estadoid, ', Nome = "', nome, '", DDD = "', ddd, '", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == ', estadoid, '), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = ', CodigoIBGE, ' },') 
            //FROM ainboxgestor.cidade
            //where(Nome in ('Rio Branco', 'Maceió', 'Macapá', 'Manaus', 'Salvador', 'Fortaleza', 'Brasília', 'Vitória', 'Goiânia', 'São Luís', 'Cuiabá', 'Campo Grande', 'Belo Horizonte', 'Belém', 'João Pessoa', 'Curitiba', 'Recife', 'Teresina', 'Rio de Janeiro', 'Natal', 'Porto Alegre', 'Porto Velho', 'Boa Vista', 'Florianópolis', 'São Paulo', 'Aracaju', 'Palmas')
            //and id not in (5282, 1664, 1656, 1267, 1271, 4157))
            //or(estadoid = 8)
            //order by estadoid;

            var cidades = new List<Cidade>
            {
                new Cidade { Id = 67, EstadoId = 1, Nome = "Rio Branco", DDD = "68", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 1), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 1200401 },
                new Cidade { Id = 1695, EstadoId = 2, Nome = "Maceió", DDD = "82", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 2), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 2704302 },
                new Cidade { Id = 303, EstadoId = 3, Nome = "Macapá", DDD = "96", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 3), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 1600303 },
                new Cidade { Id = 112, EstadoId = 4, Nome = "Manaus", DDD = "92", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 4), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 1302603 },
                new Cidade { Id = 2161, EstadoId = 5, Nome = "Salvador", DDD = "71", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 5), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 2927408 },
                new Cidade { Id = 948, EstadoId = 6, Nome = "Fortaleza", DDD = "85", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 6), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 2304400 },
                new Cidade { Id = 5564, EstadoId = 7, Nome = "Brasília", DDD = "61", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 7), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 5300108 },
                new Cidade { Id = 3096, EstadoId = 8, Nome = "Afonso Cláudio", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3200102 },
                new Cidade { Id = 3097, EstadoId = 8, Nome = "Águia Branca", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3200136 },
                new Cidade { Id = 3098, EstadoId = 8, Nome = "Água Doce do Norte", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3200169 },
                new Cidade { Id = 3099, EstadoId = 8, Nome = "Alegre", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3200201 },
                new Cidade { Id = 3100, EstadoId = 8, Nome = "Alfredo Chaves", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3200300 },
                new Cidade { Id = 3101, EstadoId = 8, Nome = "Alto Rio Novo", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3200359 },
                new Cidade { Id = 3102, EstadoId = 8, Nome = "Anchieta", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3200409 },
                new Cidade { Id = 3103, EstadoId = 8, Nome = "Apiacá", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3200508 },
                new Cidade { Id = 3104, EstadoId = 8, Nome = "Aracruz", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3200607 },
                new Cidade { Id = 3105, EstadoId = 8, Nome = "Atilio Vivacqua", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3200706 },
                new Cidade { Id = 3106, EstadoId = 8, Nome = "Baixo Guandu", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3200805 },
                new Cidade { Id = 3107, EstadoId = 8, Nome = "Barra de São Francisco", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3200904 },
                new Cidade { Id = 3108, EstadoId = 8, Nome = "Boa Esperança", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3201001 },
                new Cidade { Id = 3109, EstadoId = 8, Nome = "Bom Jesus do Norte", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3201100 },
                new Cidade { Id = 3110, EstadoId = 8, Nome = "Brejetuba", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3201159 },
                new Cidade { Id = 3111, EstadoId = 8, Nome = "Cachoeiro de Itapemirim", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3201209 },
                new Cidade { Id = 3112, EstadoId = 8, Nome = "Cariacica", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3201308 },
                new Cidade { Id = 3113, EstadoId = 8, Nome = "Castelo", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3201407 },
                new Cidade { Id = 3114, EstadoId = 8, Nome = "Colatina", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3201506 },
                new Cidade { Id = 3115, EstadoId = 8, Nome = "Conceição da Barra", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3201605 },
                new Cidade { Id = 3116, EstadoId = 8, Nome = "Conceição do Castelo", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3201704 },
                new Cidade { Id = 3117, EstadoId = 8, Nome = "Divino de São Lourenço", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3201803 },
                new Cidade { Id = 3118, EstadoId = 8, Nome = "Domingos Martins", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3201902 },
                new Cidade { Id = 3119, EstadoId = 8, Nome = "Dores do Rio Preto", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3202009 },
                new Cidade { Id = 3120, EstadoId = 8, Nome = "Ecoporanga", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3202108 },
                new Cidade { Id = 3121, EstadoId = 8, Nome = "Fundão", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3202207 },
                new Cidade { Id = 3122, EstadoId = 8, Nome = "Governador Lindenberg", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3202256 },
                new Cidade { Id = 3123, EstadoId = 8, Nome = "Guaçuí", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3202306 },
                new Cidade { Id = 3124, EstadoId = 8, Nome = "Guarapari", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3202405 },
                new Cidade { Id = 3125, EstadoId = 8, Nome = "Ibatiba", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3202454 },
                new Cidade { Id = 3126, EstadoId = 8, Nome = "Ibiraçu", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3202504 },
                new Cidade { Id = 3127, EstadoId = 8, Nome = "Ibitirama", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3202553 },
                new Cidade { Id = 3128, EstadoId = 8, Nome = "Iconha", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3202603 },
                new Cidade { Id = 3129, EstadoId = 8, Nome = "Irupi", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3202652 },
                new Cidade { Id = 3130, EstadoId = 8, Nome = "Itaguaçu", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3202702 },
                new Cidade { Id = 3131, EstadoId = 8, Nome = "Itapemirim", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3202801 },
                new Cidade { Id = 3132, EstadoId = 8, Nome = "Itarana", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3202900 },
                new Cidade { Id = 3133, EstadoId = 8, Nome = "Iúna", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3203007 },
                new Cidade { Id = 3134, EstadoId = 8, Nome = "Jaguaré", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3203056 },
                new Cidade { Id = 3135, EstadoId = 8, Nome = "Jerônimo Monteiro", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3203106 },
                new Cidade { Id = 3136, EstadoId = 8, Nome = "João Neiva", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3203130 },
                new Cidade { Id = 3137, EstadoId = 8, Nome = "Laranja da Terra", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3203163 },
                new Cidade { Id = 3138, EstadoId = 8, Nome = "Linhares", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3203205 },
                new Cidade { Id = 3139, EstadoId = 8, Nome = "Mantenópolis", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3203304 },
                new Cidade { Id = 3140, EstadoId = 8, Nome = "Marataízes", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3203320 },
                new Cidade { Id = 3141, EstadoId = 8, Nome = "Marechal Floriano", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3203346 },
                new Cidade { Id = 3142, EstadoId = 8, Nome = "Marilândia", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3203353 },
                new Cidade { Id = 3143, EstadoId = 8, Nome = "Mimoso do Sul", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3203403 },
                new Cidade { Id = 3144, EstadoId = 8, Nome = "Montanha", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3203502 },
                new Cidade { Id = 3145, EstadoId = 8, Nome = "Mucurici", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3203601 },
                new Cidade { Id = 3146, EstadoId = 8, Nome = "Muniz Freire", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3203700 },
                new Cidade { Id = 3147, EstadoId = 8, Nome = "Muqui", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3203809 },
                new Cidade { Id = 3148, EstadoId = 8, Nome = "Nova Venécia", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3203908 },
                new Cidade { Id = 3149, EstadoId = 8, Nome = "Pancas", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3204005 },
                new Cidade { Id = 3150, EstadoId = 8, Nome = "Pedro Canário", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3204054 },
                new Cidade { Id = 3151, EstadoId = 8, Nome = "Pinheiros", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3204104 },
                new Cidade { Id = 3152, EstadoId = 8, Nome = "Piúma", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3204203 },
                new Cidade { Id = 3153, EstadoId = 8, Nome = "Ponto Belo", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3204252 },
                new Cidade { Id = 3154, EstadoId = 8, Nome = "Presidente Kennedy", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3204302 },
                new Cidade { Id = 3155, EstadoId = 8, Nome = "Rio Bananal", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3204351 },
                new Cidade { Id = 3156, EstadoId = 8, Nome = "Rio Novo do Sul", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3204401 },
                new Cidade { Id = 3157, EstadoId = 8, Nome = "Santa Leopoldina", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3204500 },
                new Cidade { Id = 3158, EstadoId = 8, Nome = "Santa Maria de Jetibá", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3204559 },
                new Cidade { Id = 3159, EstadoId = 8, Nome = "Santa Teresa", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3204609 },
                new Cidade { Id = 3160, EstadoId = 8, Nome = "São Domingos do Norte", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3204658 },
                new Cidade { Id = 3161, EstadoId = 8, Nome = "São Gabriel da Palha", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3204708 },
                new Cidade { Id = 3162, EstadoId = 8, Nome = "São José do Calçado", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3204807 },
                new Cidade { Id = 3163, EstadoId = 8, Nome = "São Mateus", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3204906 },
                new Cidade { Id = 3164, EstadoId = 8, Nome = "São Roque do Canaã", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3204955 },
                new Cidade { Id = 3165, EstadoId = 8, Nome = "Serra", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3205002 },
                new Cidade { Id = 3166, EstadoId = 8, Nome = "Sooretama", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3205010 },
                new Cidade { Id = 3167, EstadoId = 8, Nome = "Vargem Alta", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3205036 },
                new Cidade { Id = 3168, EstadoId = 8, Nome = "Venda Nova do Imigrante", DDD = "28", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3205069 },
                new Cidade { Id = 3169, EstadoId = 8, Nome = "Viana", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3205101 },
                new Cidade { Id = 3170, EstadoId = 8, Nome = "Vila Pavão", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3205150 },
                new Cidade { Id = 3171, EstadoId = 8, Nome = "Vila Valério", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3205176 },
                new Cidade { Id = 3172, EstadoId = 8, Nome = "Vila Velha", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3205200 },
                new Cidade { Id = 3173, EstadoId = 8, Nome = "Vitória", DDD = "27", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 8), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3205309 },
                new Cidade { Id = 5412, EstadoId = 9, Nome = "Goiânia", DDD = "62", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 9), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 5208707 },
                new Cidade { Id = 635, EstadoId = 10, Nome = "São Luís", DDD = "98", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 10), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 2111300 },
                new Cidade { Id = 5214, EstadoId = 11, Nome = "Cuiabá", DDD = "65", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 11), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 5103403 },
                new Cidade { Id = 5118, EstadoId = 12, Nome = "Campo Grande", DDD = "67", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 12), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 5002704 },
                new Cidade { Id = 2308, EstadoId = 13, Nome = "Belo Horizonte", DDD = "31", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 13), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3106200 },
                new Cidade { Id = 170, EstadoId = 14, Nome = "Belém", DDD = "91", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 14), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 1501402 },
                new Cidade { Id = 1336, EstadoId = 15, Nome = "João Pessoa", DDD = "83", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 15), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 2507507 },
                new Cidade { Id = 4004, EstadoId = 16, Nome = "Curitiba", DDD = "41", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 16), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 4106902 },
                new Cidade { Id = 1595, EstadoId = 17, Nome = "Recife", DDD = "81", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 17), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 2611606 },
                new Cidade { Id = 881, EstadoId = 18, Nome = "Teresina", DDD = "86", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 18), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 2211001 },
                new Cidade { Id = 3241, EstadoId = 19, Nome = "Rio de Janeiro", DDD = "21", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 19), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3304557 },
                new Cidade { Id = 1162, EstadoId = 20, Nome = "Natal", DDD = "84", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 20), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 2408102 },
                new Cidade { Id = 4927, EstadoId = 21, Nome = "Porto Alegre", DDD = "51", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 21), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 4314902 },
                new Cidade { Id = 17, EstadoId = 22, Nome = "Porto Velho", DDD = "69", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 22), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 1100205 },
                new Cidade { Id = 139, EstadoId = 23, Nome = "Boa Vista", DDD = "95", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 23), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 1400100 },
                new Cidade { Id = 4397, EstadoId = 24, Nome = "Florianópolis", DDD = "48", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 24), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 4205407 },
                new Cidade { Id = 3828, EstadoId = 25, Nome = "São Paulo", DDD = "11", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 25), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 3550308 },
                new Cidade { Id = 1753, EstadoId = 26, Nome = "Aracaju", DDD = "79", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 26), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 2800308 },
                new Cidade { Id = 443, EstadoId = 27, Nome = "Palmas", DDD = "63", UsuarioCriacaoId = 1, Estado = estados.FirstOrDefault(e => e.Id == 27), DataCriacao = new DateTime(), UsuarioAlteracaoId = 1, DataAlteracao = new DateTime(), CodigoIBGE = 1721000 }
            }.AsQueryable();
            this.Cidades = new FakeDbSet<Cidade>(new TestDbAsyncEnumerable<Cidade>(cidades));

            var empresas = new List<Empresa>
            {
                new Empresa { Id = 1, CEP = "29010-100", CidadeId = 3173, Cidade = cidades.FirstOrDefault(c => c.Id == 3173), CNPJ = "22.440.680/0001-05", DataCriacao = new DateTime(), DataAlteracao = new DateTime(), NomeFantasia = "AI'n'Box Sistemas Web", UsuarioCriacaoId = 1, UsuarioAlteracaoId = 1, Tipo = (int)TipoEmpresa.Organizacao }
            }.AsQueryable();
            this.Empresas = new FakeDbSet<Empresa>(new TestDbAsyncEnumerable<Empresa>(empresas));

            var pessoas = new List<Pessoa>
            {
                new Pessoa { CPF = "089.032.447-61", Email = "leandro@ainbox.com.br", Id = 1, Nome = "Leandro Baêta Lustosa Pontes", UsuarioId = 1, Cargo = "Desenvolvedor", EmpresaId = 1, Empresa = empresas.FirstOrDefault(e => e.Id == 1), Telefone = "(27)98807-4735" }
            }.AsQueryable();
            this.Pessoas = new FakeDbSet<Pessoa>(new TestDbAsyncEnumerable<Pessoa>(pessoas));
        }

        public IDbSet<Configuracao> Configuracoes { get; set; }
        public IDbSet<Cidade> Cidades { get; set; }
        public IDbSet<Estado> Estados { get; set; }
        public IDbSet<Empresa> Empresas { get; set; }
        public IDbSet<Pessoa> Pessoas { get; set; }
        
        public int SaveChanges()
        {
            return 0;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Task.Run(() => { return 0; });
        }

        public IDbSet<T> Set<T>() where T : class, IEntity
        {
            FakeDbSet<T> obj = null;
            var props = this.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (prop.PropertyType.Equals(typeof(IDbSet<T>)))
                    obj = (FakeDbSet<T>)prop.GetValue(this);
            }
            return obj;
        }

        public DbEntityEntry<T> Entry<T>(T entity) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
