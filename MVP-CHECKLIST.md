# Checklist do MVP

## Pronto

- Estrutura em camadas: Api, Application, Domain e Infrastructure.
- PostgreSQL com EF Core e migrations.
- Cadastro de usuario e categorias padrao.
- Login JWT.
- Dashboard autenticado por usuario.
- Calculo de ciclo financeiro.
- Calculo de limite diario seguro.
- CRUD backend de categorias, rendas, contas fixas, despesas e metas.
- Lancamento rapido de despesa.
- Contribuicoes para metas no backend.
- Frontend estatico inicial.
- Tela de configuracao financeira.
- Tela de dashboard.
- Tela de lancamento rapido.
- Historico de despesas recentes.
- Cadastro/listagem de contas fixas.
- Cadastro/listagem de metas financeiras no frontend.
- Contribuicoes para metas no frontend.
- Scripts locais para iniciar a API.

## Proximos passos recomendados

1. Edicao e exclusao:
   - editar/excluir despesa;
   - editar/remover conta fixa;
   - editar/remover meta;
   - revisar impacto no dashboard.

2. Rendas no frontend:
   - registrar salario/entrada;
   - listar entradas do ciclo;
   - decidir quando usar renda padrao versus renda lancada.

3. Onboarding guiado:
   - renda mensal;
   - ciclo;
   - contas fixas;
   - primeira meta;
   - primeiro lancamento.

4. Qualidade:
   - testes automatizados;
   - tratamento padronizado de erros;
   - validacoes melhores no frontend;
   - estados de loading/erro mais claros.

5. Entrega:
   - revisar textos finais;
   - preparar build/publicacao;
   - decidir ambiente de deploy.

