# OrderAPI

Order API, projeto proposto como monografia para o curso de Analise e Desenvolvimento de Sistemas, onde o intuito é de oferecer um ambiente melhor para os alunos por meio de uma API.

O projeto surgiu no momento em que vimos uma possibilidade de diminuir as filas geradas no momento de comprar algum salgado, bedida ou doce no estabelecimento do Instituto Federal de Educação, Ciência e Tecnologia de São Paulo. 

Onde o principal ideia é que os alunos possam apenas pedir o seu pedido, e apenas ir buscar no horário que estiver disponível.


## Swagger

A partir da criação do projeto, foi criado um documento de API, que é um arquivo JSON, que contém todas as informações necessárias para a utilização da API.

<p align="center">
  <img src="Docs/Swagger.png" style="border-radius: 5px;">
</p>

## Banco de Dados

Todos os comando que vão ser executados aqui devem primeiro estar no diretorio `..\OrderAPI\OrderAPI.Data`, e para chergamos a este diretorio, tendo como base o diretorio pai do projeto, deve-se executar o comando `cd OrderAPI.Data`.

### Comandos para Migration

Para gerar uma migração nova, é necessario o comando `dotnet ef --startup-project ..\OrderAPI.API migrations add ****`, lembrando que onde se encontra os asteriscos deve ser sempre mudado para o nome da migration. Se tudo ocorrer de acordo com o esperado, deve ser escrito no console os seguintes retornos: 

```	
Build started...
Build succeeded.
Done. To undo this action, use 'ef migrations remove'
```

Para remover uma migração é necessario executar o comando `dotnet ef --startup-project ..\OrderAPI.API migrations remove`, para remover uma migration especificar é somente necessario colocar o nome da propria na frente do remove.

### Comandos para Banco

Para atualizar o banco de dados por meio de comandos, usando o entity framework, é necessario somente rodar `dotnet ef --startup-project ..\OrderAPI.API database update`.

E caso seja necesserario, é possivel dar drop do banco de dados por comando, `dotnet ef --startup-project ..\OrderAPI.API database drop`.

## Publicação do Projeto

Para publicar o projeto é necessario executar o comando `dotnet publish -c Release`, para publicar o projeto em modo de desenvolvimento é necessario executar o comando `dotnet publish -c Debug`.

Depois de executado os comandos acima é gerado varios arquivos que vão ser usados para publicar a api em algum servidor.

# Linux

O comandos que serão executados a seguir, são somente para a publicação do serviço no servidor linux. e para que eles  sejam executados, é necessario que o usuario esteja logado como root.

Algums diretorios e arquivos devem estar preparados para a publicação. como no caso do comando a seguir, o caminho do diretorio `/var/www/html/OrderAPI/` deve estar preparado para a publicação.

O nome do servico que vai ser usado para a publicação do projeto, e de gosto de quem for publicar entretanto como o projeto é chamado de orderapi, nada mais justo dar o proprio nome para os seriviço criado do linux.

Os comandos `sudo systemctl stop orderapi.service && sudo dotnet publish -c Release --output /var/www/orderapi/ && sudo systemctl start orderapi.service` são usados em sequencia para parar o serviço, publicar o projeto, e depois voltar ao serviço. Se for necessario ah tambem a possibilidade de usa-los de forma separada.

```
    sudo systemctl stop orderapi.service
    sudo dotnet publish -c Release --output /var/www/orderapi/
    sudo systemctl start orderapi.service
```
