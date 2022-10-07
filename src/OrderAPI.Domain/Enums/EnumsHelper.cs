namespace OrderAPI.Domain.Enums 
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
        UNDEFINED,
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
