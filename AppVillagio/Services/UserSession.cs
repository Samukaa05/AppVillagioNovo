namespace AppVillagio.Services;

public class UserSession
{
    // Aqui guardamos quem está logado
    public int Id { get; set; }
    public string Nome { get; set; }
	public string TipoUsuario { get; set; } // "Familia" ou "Agencia"
	public string Identificador { get; set; } // Telefone ou CNPJ
	public string Token { get; set; } // Futuro Token da API

	// Método para limpar dados (Logout)
	public void LimparSessao()
	{
		Nome = string.Empty;
		TipoUsuario = string.Empty;
		Identificador = string.Empty;
		Token = string.Empty;
	}

	// Verifica se tem alguém logado
	public bool EstaLogado => !string.IsNullOrEmpty(Identificador);
}