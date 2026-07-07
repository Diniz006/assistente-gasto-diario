# Backlog do MVP - Assistente de Gasto Diário

Documento de backlog priorizado para transformar o PRD em tarefas claras de desenvolvimento.  
Foco total no MVP manual, sem Open Finance, sem integrações bancárias e sem automações avançadas.

## 1. Autenticação e usuário

### AGD-01
**Título:** Criar conta com e-mail e senha  
**Como** novo usuário  
**Quero** criar minha conta com nome, e-mail e senha  
**Para** acessar meu ambiente financeiro com segurança  
**Prioridade:** Must Have

**Critérios de aceite**
- O sistema permite cadastro com nome, e-mail, senha e confirmação de senha.
- O e-mail deve ser único.
- A senha deve ser armazenada com hash.
- O usuário recebe feedback claro em caso de erro de validação.
- Após o cadastro, o usuário fica autenticado ou é direcionado para login/onboarding, conforme a estratégia definida.

**Observações técnicas**
- Validar força mínima de senha.
- Considerar normalização de e-mail.
- Preparar base para autenticação por sessão ou JWT.

### AGD-02
**Título:** Fazer login no sistema  
**Como** usuário cadastrado  
**Quero** entrar com e-mail e senha  
**Para** acessar meus dados financeiros  
**Prioridade:** Must Have

**Critérios de aceite**
- O usuário consegue autenticar com e-mail e senha válidos.
- Credenciais inválidas exibem mensagem amigável.
- Sessão autenticada é mantida de forma segura.
- Usuário logado consegue sair da aplicação.

**Observações técnicas**
- Definir tempo de expiração da sessão.
- Proteger rotas privadas.

### AGD-03
**Título:** Encerrar sessão  
**Como** usuário autenticado  
**Quero** sair da minha conta  
**Para** proteger meus dados em dispositivos compartilhados  
**Prioridade:** Must Have

**Critérios de aceite**
- O usuário pode sair da sessão a qualquer momento.
- Após logout, rotas privadas deixam de ser acessíveis.

**Observações técnicas**
- Incluir limpeza de token/sessão local.

## 2. Onboarding financeiro

### AGD-04
**Título:** Capturar dados financeiros iniciais no onboarding  
**Como** novo usuário  
**Quero** informar minha renda, dia de recebimento e contas iniciais  
**Para** começar a usar o cálculo do limite diário imediatamente  
**Prioridade:** Must Have

**Critérios de aceite**
- O onboarding solicita renda mensal.
- O onboarding solicita o dia de início do ciclo financeiro.
- O onboarding permite pular metas e contas fixas, se desejado.
- O onboarding pode salvar e avançar em etapas.
- Ao finalizar, o sistema leva o usuário ao dashboard principal.

**Observações técnicas**
- Pode ser implementado como wizard em etapas.
- Persistir progresso parcial para não perder dados.

### AGD-05
**Título:** Sugerir categorias padrão no onboarding  
**Como** novo usuário  
**Quero** começar com categorias prontas  
**Para** não precisar criar tudo manualmente  
**Prioridade:** Should Have

**Critérios de aceite**
- O usuário pode aceitar categorias padrão.
- O usuário pode começar sem categorias padrão e criar depois.
- As categorias padrão ficam disponíveis no sistema após a escolha.

**Observações técnicas**
- Criar seed inicial por usuário ou por modelo padrão do sistema.

## 3. Configuração do ciclo financeiro

### AGD-06
**Título:** Definir início do ciclo financeiro  
**Como** usuário  
**Quero** configurar o dia do meu ciclo financeiro  
**Para** que o limite diário reflita minha realidade de pagamento  
**Prioridade:** Must Have

**Critérios de aceite**
- O usuário consegue definir um dia de início do ciclo.
- O sistema calcula corretamente o período do ciclo atual.
- O ciclo pode iniciar em qualquer dia configurável permitido pelo sistema.

**Observações técnicas**
- Considerar ciclo mensal que atravessa dois meses.
- Calcular dias restantes com base no ciclo configurado.

### AGD-07
**Título:** Alterar configuração do ciclo financeiro  
**Como** usuário autenticado  
**Quero** editar o ciclo financeiro depois do onboarding  
**Para** ajustar meu planejamento quando necessário  
**Prioridade:** Should Have

**Critérios de aceite**
- O usuário pode editar o dia de início do ciclo.
- O sistema recalcula o dashboard após alteração.
- O histórico anterior não é perdido.

**Observações técnicas**
- Recalcular o período vigente sem apagar lançamentos antigos.

## 4. Categorias

### AGD-08
**Título:** Usar categorias padrão de despesas  
**Como** usuário  
**Quero** selecionar categorias básicas ao registrar despesas  
**Para** organizar meus gastos de forma simples  
**Prioridade:** Must Have

**Critérios de aceite**
- O sistema oferece categorias padrão.
- As categorias estão disponíveis para despesas, contas e metas quando aplicável.
- O usuário consegue ver a categoria selecionada no registro.

**Observações técnicas**
- Categorias iniciais sugeridas: alimentação, transporte, mercado, lazer, saúde, estudos, moradia, contas, compras, assinaturas, outros.

### AGD-09
**Título:** Criar e editar categorias personalizadas  
**Como** usuário  
**Quero** adicionar categorias próprias  
**Para** adaptar o app à minha realidade financeira  
**Prioridade:** Should Have

**Critérios de aceite**
- O usuário consegue criar uma categoria nova.
- O usuário consegue editar nome e cor da categoria.
- O usuário consegue excluir categoria não utilizada.

**Observações técnicas**
- Bloquear exclusão de categoria em uso, ou exigir remapeamento.

## 5. Contas fixas

### AGD-10
**Título:** Cadastrar conta fixa  
**Como** usuário  
**Quero** cadastrar contas recorrentes  
**Para** considerar esses valores no meu limite diário  
**Prioridade:** Must Have

**Critérios de aceite**
- O usuário consegue cadastrar nome, valor, vencimento, categoria e recorrência mensal.
- A conta aparece no painel de contas fixas.
- A conta impacta o cálculo do saldo disponível.

**Observações técnicas**
- Considerar campos de vencimento e status.

### AGD-11
**Título:** Marcar conta fixa como paga  
**Como** usuário  
**Quero** sinalizar quando uma conta foi paga  
**Para** manter meu controle financeiro atualizado  
**Prioridade:** Must Have

**Critérios de aceite**
- O usuário consegue alterar o status da conta para paga.
- Contas pagas deixam de ser exibidas como pendentes.
- O pagamento influencia o resumo do ciclo.

**Observações técnicas**
- Registrar data de pagamento.

### AGD-12
**Título:** Ver contas pendentes e atrasadas  
**Como** usuário  
**Quero** identificar contas abertas ou vencidas  
**Para** evitar esquecer compromissos importantes  
**Prioridade:** Must Have

**Critérios de aceite**
- O sistema mostra contas com status pendente e atrasada.
- Conta vencida sem pagamento muda para atrasada automaticamente.
- A listagem diferencia claramente os status.

**Observações técnicas**
- A atualização de status pode ocorrer ao carregar a tela ou por job simples diário.

### AGD-13
**Título:** Editar e excluir conta fixa  
**Como** usuário  
**Quero** ajustar ou remover uma conta fixa  
**Para** manter meu controle correto  
**Prioridade:** Should Have

**Critérios de aceite**
- O usuário consegue editar uma conta existente.
- O usuário consegue excluir uma conta fixa.
- Alterações refletem no cálculo do ciclo.

**Observações técnicas**
- Confirmar exclusão para evitar perdas acidentais.

## 6. Despesas

### AGD-14
**Título:** Registrar despesa manualmente  
**Como** usuário  
**Quero** adicionar uma despesa com valor, categoria e data  
**Para** acompanhar meu gasto real no ciclo  
**Prioridade:** Must Have

**Critérios de aceite**
- O usuário consegue salvar valor, descrição, categoria, data, forma de pagamento e observação opcional.
- A despesa passa a compor o total gasto do ciclo.
- O registro afeta o limite diário seguro.

**Observações técnicas**
- Validar valor positivo.
- Permitir data retroativa dentro de limites razoáveis.

### AGD-15
**Título:** Listar histórico de despesas  
**Como** usuário  
**Quero** ver minhas despesas registradas  
**Para** entender onde meu dinheiro foi gasto  
**Prioridade:** Must Have

**Critérios de aceite**
- O usuário vê a lista de despesas ordenada por data.
- Cada item exibe valor, categoria e data.
- A lista suporta visualização básica de detalhes.

**Observações técnicas**
- Paginação ou rolagem infinita pode ser adicionada depois, se necessário.

### AGD-16
**Título:** Editar e excluir despesa  
**Como** usuário  
**Quero** corrigir ou remover um lançamento  
**Para** manter meu histórico confiável  
**Prioridade:** Should Have

**Critérios de aceite**
- O usuário consegue alterar uma despesa existente.
- O usuário consegue excluir uma despesa.
- O dashboard recalcula os valores após alteração.

**Observações técnicas**
- Exigir confirmação na exclusão.

### AGD-17
**Título:** Filtrar despesas por data e categoria  
**Como** usuário  
**Quero** encontrar lançamentos específicos  
**Para** revisar rapidamente meus gastos  
**Prioridade:** Should Have

**Critérios de aceite**
- O usuário consegue filtrar por período.
- O usuário consegue filtrar por categoria.
- Os filtros podem ser combinados.

**Observações técnicas**
- Não precisa de filtros avançados no primeiro corte.

## 7. Metas financeiras

### AGD-18
**Título:** Criar meta financeira  
**Como** usuário  
**Quero** cadastrar uma meta com valor alvo e prazo  
**Para** acompanhar meu progresso financeiro  
**Prioridade:** Must Have

**Critérios de aceite**
- O usuário consegue cadastrar nome, valor alvo, valor atual, valor mensal planejado, prazo e prioridade.
- A meta aparece na área de metas do sistema.
- A meta influencia o saldo disponível do ciclo.

**Observações técnicas**
- O valor atual pode começar em zero.

### AGD-19
**Título:** Registrar aporte em meta  
**Como** usuário  
**Quero** adicionar valor guardado em uma meta  
**Para** ver meu progresso atualizado  
**Prioridade:** Must Have

**Critérios de aceite**
- O usuário consegue aumentar o valor atual da meta.
- O progresso é recalculado após cada aporte.
- O sistema indica quando a meta foi concluída.

**Observações técnicas**
- Pode ser um lançamento incremental ou edição direta do valor atual.

### AGD-20
**Título:** Visualizar progresso das metas  
**Como** usuário  
**Quero** ver o percentual de progresso das minhas metas  
**Para** entender se estou no caminho certo  
**Prioridade:** Must Have

**Critérios de aceite**
- O sistema mostra percentual de progresso.
- O sistema mostra valor atual e valor alvo.
- Meta concluída é destacada visualmente.

**Observações técnicas**
- Fórmula: valor atual / valor alvo * 100.

### AGD-21
**Título:** Editar e excluir meta  
**Como** usuário  
**Quero** ajustar ou remover uma meta  
**Para** manter meu planejamento atualizado  
**Prioridade:** Should Have

**Critérios de aceite**
- O usuário consegue editar meta existente.
- O usuário consegue excluir meta.
- O sistema recalcula os resumos após alterações.

**Observações técnicas**
- Confirmar exclusão de metas em andamento.

## 8. Dashboard principal

### AGD-22
**Título:** Exibir dashboard com resumo principal  
**Como** usuário autenticado  
**Quero** ver meu panorama financeiro logo ao entrar  
**Para** decidir rapidamente quanto posso gastar hoje  
**Prioridade:** Must Have

**Critérios de aceite**
- O dashboard exibe limite diário seguro.
- O dashboard exibe saldo disponível no ciclo.
- O dashboard exibe dias restantes.
- O dashboard exibe contas pendentes.
- O dashboard exibe progresso das metas.

**Observações técnicas**
- Esta é a tela mais importante do MVP.

### AGD-23
**Título:** Exibir total já gasto no ciclo  
**Como** usuário  
**Quero** ver quanto já gastei no período atual  
**Para** entender meu ritmo de consumo  
**Prioridade:** Must Have

**Critérios de aceite**
- O sistema mostra o total gasto do ciclo.
- O valor considera apenas despesas registradas.
- O total é atualizado ao incluir, editar ou excluir despesas.

**Observações técnicas**
- Deve usar a data dentro do ciclo financeiro atual.

### AGD-24
**Título:** Exibir categorias com maior consumo  
**Como** usuário  
**Quero** saber onde gasto mais  
**Para** identificar meus hábitos de consumo  
**Prioridade:** Could Have

**Critérios de aceite**
- O dashboard mostra pelo menos as categorias de maior gasto.
- O cálculo é simples e derivado das despesas lançadas.

**Observações técnicas**
- Pode começar como lista, sem gráfico.

## 9. Cálculo do gasto diário seguro

### AGD-25
**Título:** Calcular gasto diário seguro automaticamente  
**Como** usuário  
**Quero** que o sistema calcule meu limite diário  
**Para** saber quanto posso gastar hoje sem comprometer o mês  
**Prioridade:** Must Have

**Critérios de aceite**
- O sistema calcula saldo disponível do ciclo.
- O sistema divide o saldo disponível pelos dias restantes.
- O resultado é exibido no dashboard.
- Se o saldo disponível for zero ou negativo, o limite diário vira zero.

**Observações técnicas**
- Fórmula base:
  - saldo_disponivel = renda do ciclo - contas fixas - metas planejadas - despesas realizadas
  - gasto_diario_seguro = saldo_disponivel / dias restantes

### AGD-26
**Título:** Recalcular limite diário após mudanças financeiras  
**Como** usuário  
**Quero** que o limite diário se ajuste quando eu registrar ou alterar dados  
**Para** manter a recomendação sempre atualizada  
**Prioridade:** Must Have

**Critérios de aceite**
- O limite muda quando uma despesa é criada, editada ou removida.
- O limite muda quando uma conta fixa é paga ou alterada.
- O limite muda quando uma meta recebe aporte.

**Observações técnicas**
- Recalcular de forma determinística e simples.

## 10. Alertas simples

### AGD-27
**Título:** Alertar quando o gasto do dia ultrapassar o limite  
**Como** usuário  
**Quero** receber um aviso quando gasto demais  
**Para** evitar ultrapassar o orçamento diário ideal  
**Prioridade:** Must Have

**Critérios de aceite**
- O sistema compara gasto do dia com o limite diário seguro.
- Se o gasto exceder o limite, uma mensagem de alerta aparece.
- O alerta usa linguagem simples e acolhedora.

**Observações técnicas**
- O alerta pode aparecer no dashboard e na confirmação do lançamento.

### AGD-28
**Título:** Alertar quando o saldo do ciclo estiver baixo  
**Como** usuário  
**Quero** ver um aviso de risco financeiro  
**Para** agir antes de ficar sem dinheiro  
**Prioridade:** Should Have

**Critérios de aceite**
- O sistema alerta quando o saldo livre estiver abaixo de um patamar mínimo.
- A mensagem é clara e não punitiva.

**Observações técnicas**
- O patamar pode ser configurado depois; no MVP, pode ser fixo.

### AGD-29
**Título:** Mostrar mensagens positivas quando houver folga no orçamento  
**Como** usuário  
**Quero** receber feedback positivo quando gasto menos  
**Para** me sentir motivado a continuar controlando meu dinheiro  
**Prioridade:** Could Have

**Critérios de aceite**
- O sistema pode exibir mensagens de reforço positivo.
- A mensagem aparece com base no comportamento do ciclo.

**Observações técnicas**
- Não deve ser requisito para o funcionamento do MVP.

## 11. Resumo mensal

### AGD-30
**Título:** Ver resumo mensal do ciclo atual  
**Como** usuário  
**Quero** visualizar um resumo consolidado do mês/ciclo  
**Para** entender meu fechamento financeiro  
**Prioridade:** Should Have

**Critérios de aceite**
- O resumo mostra renda do ciclo.
- O resumo mostra total gasto.
- O resumo mostra total de contas.
- O resumo mostra total reservado para metas.
- O resumo mostra saldo final.

**Observações técnicas**
- Pode ser uma tela simples, sem gráficos.

### AGD-31
**Título:** Visualizar status consolidado do ciclo  
**Como** usuário  
**Quero** ver se meu ciclo está saudável ou comprometido  
**Para** tomar decisões com base no saldo restante  
**Prioridade:** Should Have

**Critérios de aceite**
- O sistema informa se o ciclo está confortável, em atenção ou comprometido.
- O status é baseado em regras simples de saldo disponível.

**Observações técnicas**
- Exibir apenas uma classificação resumida no MVP.

### AGD-32
**Título:** Comparar ciclos em versão simplificada  
**Como** usuário  
**Quero** comparar meu resultado com o ciclo anterior  
**Para** acompanhar evolução financeira  
**Prioridade:** Could Have

**Critérios de aceite**
- O usuário pode ver uma comparação simples entre ciclos.
- A comparação pode ser textual, sem necessidade de gráficos.

**Observações técnicas**
- Pode ficar para fase posterior se não houver tempo.

## 12. Responsividade e experiência do usuário

### AGD-33
**Título:** Adaptar o layout para celular e desktop  
**Como** usuário  
**Quero** usar o sistema em telas diferentes  
**Para** acessar meu controle financeiro em qualquer dispositivo  
**Prioridade:** Must Have

**Critérios de aceite**
- O layout funciona bem em desktop e celular.
- Os formulários continuam legíveis em telas menores.
- O dashboard principal permanece utilizável em mobile.

**Observações técnicas**
- O MVP deve ser web responsivo, não app nativo.

### AGD-34
**Título:** Facilitar cadastro rápido de despesas  
**Como** usuário  
**Quero** registrar gastos com poucos cliques  
**Para** não desistir de usar o sistema no dia a dia  
**Prioridade:** Must Have

**Critérios de aceite**
- O fluxo de nova despesa é curto e objetivo.
- O usuário consegue salvar uma despesa sem navegar por muitas telas.

**Observações técnicas**
- Considerar modal, drawer ou página dedicada simples.

### AGD-35
**Título:** Exibir mensagens simples e acolhedoras  
**Como** usuário  
**Quero** receber feedback claro do sistema  
**Para** entender minha situação financeira sem fricção  
**Prioridade:** Should Have

**Critérios de aceite**
- As mensagens evitam tom agressivo.
- O sistema explica o limite de forma simples.
- Erros de validação são fáceis de entender.

**Observações técnicas**
- Usar uma linguagem brasileira, curta e direta.

### AGD-36
**Título:** Manter navegação consistente entre telas  
**Como** usuário  
**Quero** encontrar rapidamente as principais áreas do sistema  
**Para** não me perder durante o uso  
**Prioridade:** Should Have

**Critérios de aceite**
- O menu ou navegação principal é consistente.
- As telas principais são acessíveis em poucos cliques.

**Observações técnicas**
- Estrutura recomendada: Dashboard, Despesas, Contas, Metas, Resumo, Configurações.

## Futuro / Pós-MVP

Itens explicitamente fora do backlog inicial, mas relevantes para roadmap:

- Open Finance real.
- Conexão com bancos.
- Conexão com cartões.
- Pix.
- OCR.
- Leitura de fatura por foto.
- Importação automática de extratos.
- IA obrigatória para interpretação de texto.
- App mobile nativo.
- Notificações push.
- Pagamentos e transferências.
- Categorização automática avançada.
- Detecção de assinaturas.
- Cofrinho automático.
- Arredondamento de compras.
- Relatórios avançados com automações.

## Sugestão de ordem de implementação

1. Autenticação e usuário.
2. Configuração do ciclo financeiro.
3. Onboarding financeiro.
4. Categorias padrão.
5. Contas fixas.
6. Despesas.
7. Metas financeiras.
8. Cálculo do gasto diário seguro.
9. Dashboard principal.
10. Alertas simples.
11. Resumo mensal.
12. Responsividade e ajustes finais de UX.

