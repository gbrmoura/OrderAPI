namespace OrderAPI.Data.Helpers 
{
    public enum PrevilegioEnum 
    {
        MASTER, 
        GERENTE,
        FUNCIONARIO,
        USUARIO
    }

    public enum PedidoStatusEnum 
    {
        ABERTO,
        RETIRADO,
        CANCELADO
    }

    public enum EstoqueCrontoleEnum
    {
        ENTRADA,
        SAIDA
    }
}
