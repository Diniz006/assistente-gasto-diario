# PRD - MVP do Assistente de Gasto Diario

## 1. Nome provisório do produto

**Assistente de Gasto Diario**

Nomes alternativos para decisão futura:

- Saldo Vivo
- Bolso Diario
- Meu Limite Diario
- Grana no Controle
- Financas Sem Culpa

## 2. Resumo do produto

O produto sera um sistema web de finanças pessoais que ajuda o usuario a responder uma pergunta simples:

**"Quanto eu posso gastar hoje sem comprometer minhas contas, minhas metas e meu mes?"**

Diferente de uma planilha ou de um app financeiro cheio de graficos, o foco do MVP sera dar uma resposta diaria clara, simples e pratica.

O usuario cadastra sua renda, contas fixas, metas financeiras e despesas. A partir disso, o sistema calcula automaticamente um **limite diario seguro de gasto**.

A primeira versao sera manual, sem conexao bancaria, sem Open Finance e sem automacoes complexas.

## 3. Referencias de produto

Este MVP sera inspirado em dois aplicativos estudados:

### Thor: Budget & Expenses

Referencia principal para:

- proposta simples;
- tom amigavel;
- foco em "quanto posso gastar hoje?";
- experiencia emocional;
- assistente financeiro pessoal;
- metas com previsao;
- registro rapido de despesas.

O Thor e a principal referencia de **posicionamento e experiencia do usuario**.

### Oinc - Financas Pessoais

Referencia principal para:

- automacao futura;
- categorizacao de gastos;
- orcamento mensal;
- centralizacao financeira;
- relatorios;
- cofrinho;
- recursos avancados com Open Finance.

O Oinc e a principal referencia de **roadmap e maturidade futura**.

A decisao estrategica e comecar mais perto do Thor, pois ele e mais simples e viavel para MVP, deixando ideias do Oinc para versoes futuras.

## 4. Problema

Muitas pessoas tem dificuldade em controlar o dinheiro porque nao sabem exatamente quanto podem gastar no dia a dia.

Elas ate sabem quanto ganham, mas se perdem em:

- contas fixas;
- parcelas;
- gastos pequenos;
- cartao;
- metas;
- compras impulsivas;
- falta de planejamento ate o fim do mes.

O problema principal nao e apenas registrar gastos. O problema e tomar decisoes diarias com clareza.

A pergunta real do usuario e:

**"Posso gastar isso hoje ou vou me apertar depois?"**

## 5. Publico-alvo inicial

O MVP sera pensado principalmente para:

- jovens iniciantes na vida financeira;
- estudantes;
- pessoas com renda baixa ou media;
- pessoas que recebem salario mensal;
- pessoas que querem sair do descontrole financeiro;
- pessoas que acham planilhas complicadas;
- pessoas que querem uma resposta simples, nao um relatorio complexo.

## 6. Proposta de valor

O Assistente de Gasto Diario ajuda o usuario a saber quanto pode gastar hoje de forma segura, considerando:

- renda mensal;
- contas fixas;
- despesas ja realizadas;
- metas financeiras;
- dias restantes no ciclo financeiro.

A proposta de valor principal e:

**"Saiba quanto voce pode gastar hoje sem culpa e sem comprometer seu mes."**

## 7. Objetivo do MVP

Validar se um sistema simples de limite diario financeiro e util o suficiente para o usuario.

O MVP precisa provar que:

1. O usuario entende facilmente o conceito de limite diario.
2. O calculo ajuda na tomada de decisao.
3. O cadastro manual nao e uma barreira grande no inicio.
4. O produto e mais claro do que uma planilha comum.
5. A tela principal entrega valor rapidamente.

## 8. O que o MVP tera

A primeira versao tera:

1. Cadastro de usuario.
2. Login.
3. Cadastro de renda mensal.
4. Cadastro de ciclo financeiro.
5. Cadastro de contas fixas.
6. Cadastro de metas financeiras.
7. Cadastro de despesas.
8. Cadastro de categorias.
9. Dashboard principal.
10. Calculo de gasto diario seguro.
11. Resumo do mes.
12. Alertas simples.
13. Status de contas: pendente, paga ou atrasada.
14. Progresso de metas.
15. Historico de transacoes.

## 9. O que o MVP nao tera

A primeira versao nao tera:

- Open Finance;
- conexao com bancos;
- conexao com cartoes;
- pagamentos;
- transferencias;
- leitura automatica de extratos;
- leitura de fatura por foto;
- OCR;
- IA obrigatoria;
- aplicativo mobile nativo;
- notificacoes push;
- categorizacao automatica avancada;
- multiusuario familiar;
- investimentos;
- emissao de boletos;
- integracao com Pix.

Essas funcionalidades podem ser consideradas em versoes futuras.

## 10. Funcionalidade principal

A funcionalidade central sera o calculo do **gasto diario seguro**.

### Formula inicial

```txt
saldo_disponivel = renda_do_ciclo - contas_fixas_pendentes_ou_planejadas - valor_planejado_para_metas - despesas_realizadas

dias_restantes = dias_restantes_no_ciclo_financeiro

gasto_diario_seguro = saldo_disponivel / dias_restantes
```

### Exemplo

```txt
Renda mensal: R$ 2.000
Contas fixas do mes: R$ 900
Valor planejado para metas: R$ 300
Despesas ja realizadas: R$ 200
Dias restantes no ciclo: 20

Saldo disponivel = 2000 - 900 - 300 - 200
Saldo disponivel = R$ 600

Gasto diario seguro = 600 / 20
Gasto diario seguro = R$ 30
```

Mensagem exibida ao usuario:

```txt
Hoje voce pode gastar ate R$ 30,00 sem comprometer suas contas e metas.
```

## 11. Ciclo financeiro

O sistema nao deve depender apenas do mes-calendario.

O usuario podera configurar o dia de inicio do ciclo financeiro, por exemplo:

- dia 1;
- dia 5;
- dia 10;
- dia 15;
- dia do pagamento.

Isso e importante porque muitas pessoas nao recebem exatamente no primeiro dia do mes.

### Exemplo

Se o usuario recebe no dia 10, o ciclo financeiro pode ser:

```txt
10 de julho ate 9 de agosto
```

Em vez de:

```txt
1 de julho ate 31 de julho
```

## 12. Telas do MVP

### 12.1 Tela de cadastro

Campos:

- nome;
- e-mail;
- senha;
- confirmacao de senha.

### 12.2 Tela de login

Campos:

- e-mail;
- senha.

### 12.3 Onboarding financeiro

Primeiro fluxo apos cadastro.

Perguntas:

1. Quanto voce recebe por mes?
2. Em qual dia voce recebe?
3. Voce quer definir uma meta financeira agora?
4. Quais contas fixas voce tem?
5. Voce quer comecar com categorias padrao?

Objetivo: deixar o usuario pronto para ver o dashboard.

### 12.4 Dashboard principal

Esta sera a tela mais importante do produto.

Deve mostrar:

- limite seguro para gastar hoje;
- saldo disponivel no ciclo;
- dias restantes no ciclo;
- total de contas pendentes;
- total ja gasto no ciclo;
- progresso das metas;
- principais categorias de gasto;
- alerta principal do dia.

Exemplo de mensagem principal:

```txt
Hoje voce pode gastar R$ 30,00.
Voce ainda tem R$ 600,00 livres ate o fim do ciclo.
```

Se o usuario estiver em risco:

```txt
Atencao: seu saldo livre esta baixo.
Seu limite seguro hoje e R$ 8,50.
```

Se o saldo estiver negativo:

```txt
Seu orcamento esta comprometido.
Evite novos gastos ate ajustar suas contas.
```

### 12.5 Tela de despesas

Funcoes:

- listar despesas;
- adicionar nova despesa;
- editar despesa;
- excluir despesa;
- filtrar por data;
- filtrar por categoria.

Campos da despesa:

- valor;
- descricao;
- categoria;
- data;
- forma de pagamento;
- observacao.

### 12.6 Tela de contas fixas

Funcoes:

- cadastrar conta fixa;
- marcar como paga;
- editar conta;
- excluir conta;
- visualizar contas pendentes;
- visualizar contas atrasadas.

Campos da conta fixa:

- nome;
- valor;
- vencimento;
- categoria;
- recorrencia mensal;
- status.

Status possiveis:

- pendente;
- paga;
- atrasada.

### 12.7 Tela de metas

Funcoes:

- criar meta;
- editar meta;
- excluir meta;
- adicionar valor guardado;
- acompanhar progresso.

Campos da meta:

- nome;
- valor alvo;
- valor atual;
- valor mensal planejado;
- prazo desejado;
- prioridade.

Exemplo:

```txt
Meta: Comprar notebook
Valor alvo: R$ 3.000
Valor atual: R$ 450
Progresso: 15%
```

### 12.8 Tela de categorias

Categorias padrao:

- alimentacao;
- transporte;
- mercado;
- lazer;
- saude;
- estudos;
- moradia;
- contas;
- compras;
- assinaturas;
- outros.

O usuario podera criar categorias proprias.

### 12.9 Tela de resumo mensal

Deve mostrar:

- renda do ciclo;
- total gasto;
- total em contas;
- total reservado para metas;
- saldo final;
- categoria com maior gasto;
- comparacao simples com ciclo anterior, em versao futura.

### 12.10 Tela de configuracoes

Deve permitir alterar:

- nome;
- e-mail;
- senha;
- dia de inicio do ciclo financeiro;
- moeda;
- categorias;
- preferencias basicas.

## 13. Entidades principais

### Usuario

Campos:

- id;
- nome;
- e-mail;
- senha_hash;
- data_criacao;
- data_atualizacao.

### Configuracao financeira

Campos:

- id;
- usuario_id;
- renda_mensal_padrao;
- dia_inicio_ciclo;
- moeda;
- data_criacao;
- data_atualizacao.

### Receita

Campos:

- id;
- usuario_id;
- descricao;
- valor;
- data_recebimento;
- recorrente;
- categoria;
- data_criacao.

### Despesa

Campos:

- id;
- usuario_id;
- categoria_id;
- descricao;
- valor;
- data_despesa;
- forma_pagamento;
- observacao;
- data_criacao;
- data_atualizacao.

### Conta fixa

Campos:

- id;
- usuario_id;
- categoria_id;
- nome;
- valor;
- dia_vencimento;
- recorrente;
- status;
- data_pagamento;
- data_criacao;
- data_atualizacao.

### Categoria

Campos:

- id;
- usuario_id;
- nome;
- tipo;
- cor;
- padrao;
- data_criacao.

Tipos possiveis:

- receita;
- despesa;
- conta;
- meta.

### Meta financeira

Campos:

- id;
- usuario_id;
- nome;
- valor_alvo;
- valor_atual;
- valor_mensal_planejado;
- data_limite;
- prioridade;
- status;
- data_criacao;
- data_atualizacao.

Status possiveis:

- ativa;
- concluida;
- pausada;
- cancelada.

### Orcamento mensal

Campos:

- id;
- usuario_id;
- categoria_id;
- valor_limite;
- ciclo_inicio;
- ciclo_fim;
- data_criacao.

### Alerta

Campos:

- id;
- usuario_id;
- tipo;
- mensagem;
- lido;
- data_criacao.

## 14. Regras de negocio

### 14.1 Calculo do saldo disponivel

```txt
saldo_disponivel = receitas_do_ciclo - contas_fixas_do_ciclo - metas_planejadas_do_ciclo - despesas_do_ciclo
```

### 14.2 Calculo do gasto diario seguro

```txt
gasto_diario_seguro = saldo_disponivel / dias_restantes
```

Se o saldo disponivel for menor ou igual a zero:

```txt
gasto_diario_seguro = 0
```

### 14.3 Dias restantes

O calculo deve considerar o ciclo financeiro do usuario.

Exemplo:

```txt
Ciclo: 10/07 ate 09/08
Data atual: 20/07
Dias restantes: 21
```

### 14.4 Status de conta fixa

Uma conta fixa pode ter tres status:

```txt
pendente
paga
atrasada
```

Regra:

- se a conta nao foi paga e a data atual passou do vencimento, status = atrasada;
- se foi paga, status = paga;
- se ainda nao venceu, status = pendente.

### 14.5 Progresso de meta

```txt
progresso_meta = valor_atual / valor_alvo * 100
```

Se o progresso for maior ou igual a 100%, a meta pode ser marcada como concluida.

### 14.6 Alerta de excesso de gasto

Se o gasto do dia for maior que o gasto diario seguro:

```txt
alerta = "Voce passou do limite ideal de hoje."
```

### 14.7 Alerta de saldo comprometido

Se o saldo disponivel for menor que 10% da renda do ciclo:

```txt
alerta = "Seu saldo livre esta baixo para o restante do ciclo."
```

### 14.8 Fechamento de ciclo

Ao fim de um ciclo financeiro, o sistema deve:

- salvar resumo do ciclo;
- calcular saldo final;
- manter historico;
- preparar novo ciclo;
- gerar contas recorrentes do proximo ciclo.

No MVP, essa regra pode ser simples e manual. Em versoes futuras, pode ser automatica.

## 15. Mensagens do sistema

O tom deve ser simples, direto e acolhedor.

Exemplos:

```txt
Hoje voce pode gastar ate R$ 30,00 sem comprometer seu mes.
```

```txt
Boa! Voce ficou R$ 12,00 abaixo do limite de hoje.
```

```txt
Atencao: voce passou R$ 18,50 do limite seguro de hoje.
```

```txt
Sua meta esta avancando. Voce ja completou 35%.
```

```txt
Se continuar nesse ritmo, pode faltar dinheiro antes do fim do ciclo.
```

```txt
Voce tem 3 contas pendentes este mes.
```

## 16. Metricas de sucesso do MVP

O MVP sera considerado bem-sucedido se conseguir demonstrar:

1. O usuario consegue configurar sua vida financeira inicial em poucos minutos.
2. O usuario entende o limite diario calculado.
3. O usuario consegue cadastrar uma despesa rapidamente.
4. O dashboard responde claramente quanto ele pode gastar hoje.
5. O sistema mantem historico organizado.
6. O projeto fica apresentavel como portifolio profissional.

Metricas futuras:

- numero de usuarios cadastrados;
- numero de despesas registradas por usuario;
- frequencia de uso semanal;
- quantidade de metas criadas;
- porcentagem de usuarios que completam o onboarding;
- tempo medio para cadastrar primeira despesa.

## 17. Criterios de aceite do MVP

O MVP estara pronto quando:

- o usuario conseguir criar conta;
- o usuario conseguir fazer login;
- o usuario conseguir informar renda e dia de inicio do ciclo;
- o usuario conseguir cadastrar contas fixas;
- o usuario conseguir cadastrar despesas;
- o usuario conseguir cadastrar metas;
- o dashboard calcular o gasto diario seguro;
- o sistema mostrar saldo disponivel;
- o sistema mostrar contas pendentes;
- o sistema mostrar progresso de metas;
- o usuario conseguir visualizar historico de despesas;
- o layout estiver responsivo para desktop e celular.

## 18. Roadmap

### Fase 1 - MVP manual

- cadastro;
- login;
- renda;
- contas fixas;
- despesas;
- metas;
- dashboard;
- limite diario seguro.

### Fase 2 - Relatorios

- graficos;
- comparacao mensal;
- gastos por categoria;
- evolucao de metas;
- resumo financeiro mais completo.

### Fase 3 - Assistente conversacional

- registro por texto;
- interpretacao de frases simples;
- explicacao do limite diario;
- mensagens personalizadas.

Exemplo:

```txt
Gastei R$ 35 no mercado hoje.
```

O sistema interpreta:

```txt
tipo: despesa
valor: 35
categoria: mercado
data: hoje
```

### Fase 4 - Importacao de arquivos

- importacao CSV;
- importacao XLSX;
- importacao de extratos simples;
- categorizacao semiautomatica.

### Fase 5 - Automacoes

- deteccao de assinaturas;
- sugestoes de economia;
- orcamento por categoria;
- alertas inteligentes;
- recorrencias automaticas.

### Fase 6 - Open Finance

- conexao bancaria;
- saldos automaticos;
- transacoes automaticas;
- categorizacao baseada em dados reais;
- consolidacao financeira completa.

## 19. Diferenciais proprios

O produto nao deve tentar ser uma copia do Thor nem do Oinc.

Diferenciais desejados:

- linguagem brasileira e simples;
- foco em jovens iniciantes;
- modo "sobrevivencia financeira";
- resposta diaria objetiva;
- metas pequenas e realistas;
- explicacao clara dos calculos;
- pouca friccao;
- ciclo financeiro configuravel;
- visual limpo;
- tom educativo, nao punitivo;
- bom valor como projeto de portifolio.

## 20. Decisao estrategica

A primeira versao deve ser pequena, mas bem pensada.

O erro seria tentar comecar com um app financeiro completo.

A decisao correta e comecar com uma pergunta forte:

**"Quanto posso gastar hoje?"**

Se o produto responder isso bem, ele ja tera valor.

Depois, ele pode evoluir para relatorios, assistente, importacao de arquivos, automacoes e, futuramente, Open Finance.
