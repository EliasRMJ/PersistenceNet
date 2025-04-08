namespace PersistenceNet.MessagesProvider.Locations
{
    public class MessagesPtBr : Messages
    {
        public override string Error => "ERRO";
        public override string Warning => "ATENÇÃO";
        public override string Success => "SUCESSO";
        public override string Unknown => "[desconhecido]";

        public override string BadRequest => "Ocorreu uma exceção de aplicativo.";
        public override string NotFound => "A chave solicitada não foi encontrada.";
        public override string Unauthorized => "Acesso não autorizado.";
        public override string InternalServerError => "Erro interno do servidor. Por favor, tente novamente mais tarde.";
        public override string UnexpectedOccurred => "Ocorreu um erro inesperado.";

        public override string EntityNull => "A entidade é nula";
        public override string RegisterSuccess => "Registro concluído com sucesso!";
        public override string UpdateSuccess => "Atualização executada com êxito!";
        public override string DeleteSuccess => "Exclusão executada com êxito!";
        public override string AddSuccess => "adicionado com sucesso!";
        public override string IncludeProblem => "Ops, algo deu errado ao incluir a entidade!";
        public override string UpdateProblem => "Ops, algo deu errado ao atualizar a entidade!";
        public override string DeleteProblem => "Ops, algo deu errado ao excluir a entidade!";
        public override string UnexpectedError => "Ocorreu um erro inesperado na exclusão da entidade";
        public override string CheckProperty => "Verifica se a propriedade é uma primeira chave ou uma chave estrangeira.";
        public override string PropertyRequired => "obrigatório(a), mas nenhuma mensagem configurada na entidade!";
        public override string EntityFound => "Entidade não encontrada!";

        public override string EntityConversion => "Iniciando a conversão de objeto em entidade no método";
        public override string StartCallMethod => "Convertendo o objeto para a entidade executada com sucesso no método";

        public override string NoResultList => "Nenhum registro encontrado neste momento com os filtros fornecidos!";
        public override string NoResult => "Nenhum registro encontrado!";

        public override string FilterMethod => "Iniciando o método de filtros em";
        public override string ConvertMethodFilter => "Convertendo a entidade para o objeto executado com êxito no método de filtro.";

        public override string TransactionErrorUnexpected => "Erro inesperado ao concluir a transação";
        public override string TransactionError => "Erro ao iniciar a transação!";
        public override string TransactionNoStarting => "O 'CommitAsync' precisa de uma transação ativa. Tente iniciar o método primeiro 'BeginTransactionAsync'!";
    }
}