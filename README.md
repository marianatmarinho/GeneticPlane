#  geneticplane
algoritmo genético para encontrar a melhor aeronave que supra as exigências da competição nacional de Aerodesign, onde equipes de estudantes devem construir um avião cargueiro radio controlado que seja o mais leve possível e capaz de carregar uma grande quantidade de carga.

### Começando
Para executar o projeto, será necessário instalar os seguintes programas:
- [C# Visual Studio: para desenvolvimento do projeto](https://visualstudio.microsoft.com/vs/)
- [Anaconda: necessário para executar o script Pyhton](https://www.anaconda.com/products/individual)
- [VTK: necessário para visualização de arquivos com imagem 3D](https://vtk.org/download/)

### Desenvolvimento
Para iniciar o desenvolvimento, é necessário clonar o projeto do GitHub no diretório de sua preferência

```
cd "diretorio de sua preferencia"
git clone "link do projeto"
```
### Features
A aplicação é um algoritmo Genético (AG) para selecionar o melhor avião para o problema proposto. A aplicação é desktop e em linguagem C#. Ela recebe como parâmetros intervalos de valores referentes às características de uma aeronave. o AG seleciona o melhor avião para o problema proposto com base em sua pontuação (definida anualmente pelo Aerodesign). Após o processamento, é gerado a imagem do avião resultante do AG e visualizada através do VTK. Essa imagem do avião é gerada transformando o perfil 2D do Airfoil Database em 3D através de um código em Python.

![](/genetic_plane.jpg)

### Configuração
Para executar o projeto, é necessário utilizar o Visual Studio, para que o mesmo identifique as dependências necessárias para a execução. Além disso, é necessário instalar o VTK e o Anaconda pra geração do avião obtido no C#.

### Contribuições
Contribuições são sempre bem-vindas! Para contribuir lembre-se sempre de adicionar testes unitários para as novas classes com a devida documentação.

### Licença
Não se aplica.
