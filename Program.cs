using System;

namespace TicTacToe;

class Program{
  static void Main(){
    Game game = Game.GetGame;
    game.ShowMenu();
  }
}
class Player{
  public string Name { get; private set; } = "";
  public int Score { get; private set; } = 0;
  public Player(string name){
    Name = name;
  }
  public void IncrementScore(){
    Score ++;
  }
  public void ChangeName(string newName){
    Name = newName;
  }
}
class Game{
  private Game(){ }
  private static Game? s_game { get; set; } = null;
  public static Game GetGame {
    get{
      if(s_game is null){
        s_game = new Game();
      }
      return s_game;
    }
  }
  private Player? _firstPlayer { get; set; } = null;
  private Player? _secondPlayer { get; set; } = null;
  private char[,] _gameField { get; set; } = {
    {' ', '|', ' ', '|', ' '},
    {'-', '-', '-', '-', '-'},
    {' ', '|', ' ', '|', ' '},
    {'-', '-', '-', '-', '-'},
    {' ', '|', ' ', '|', ' '}
  };
  private void _ClearGameField(){
    for(var i = 0; i < 5; i += 2){
      for(var j = 0; j < 5; j += 2){
        _gameField[i, j] = ' ';
      }
    }
  }
  private void _InitializeGameField(){
    for(var i = 0; i < 5; i ++){
      for(var j = 0; j < 5; j ++){
        Console.Write(_gameField[i, j]);
      }
      Console.Write('\n');
    }
  }
  private char symbol = 'X';
  private bool _IsEmpty(byte posY, byte posX){
    return _gameField[posY, posX] == ' ';
  }
  private bool _isGameOver = false;
  private bool _isFirstPlayer = true;
  private byte _fiveTimesMove = 0;
  private void _IsGameOver(){
    if((_gameField[0, 0] == symbol && _gameField[0, 2] == symbol && _gameField[0, 4] == symbol) ||
    (_gameField[2, 0] == symbol && _gameField[2, 2] == symbol && _gameField[2, 4] == symbol) || (_gameField[4, 0] == symbol && _gameField[4, 2] == symbol && _gameField[4, 4] == symbol) || (_gameField[0, 0] == symbol && _gameField[2, 2] == symbol && _gameField[4, 4] == symbol) ||
    (_gameField[0, 4] == symbol && _gameField[2, 2] == symbol && _gameField[4, 0] == symbol) ||
    (_gameField[0, 0] == symbol && _gameField[2, 0] == symbol && _gameField[4, 0] == symbol) ||
    (_gameField[0, 2] == symbol && _gameField[2, 2] == symbol && _gameField[4, 2] == symbol) ||
    (_gameField[0, 4] == symbol && _gameField[2, 4] == symbol && _gameField[4, 4] == symbol)){
      _isGameOver = true;
      _fiveTimesMove = 0;
      if(_isFirstPlayer){
        _firstPlayer!.IncrementScore();
        _isFirstPlayer = false;
      }
      else{
        _secondPlayer!.IncrementScore();
        _isFirstPlayer = true;
      }
    }
  }
  private char[] _unacceptableChars = new char[] { '#', '?', '!', '.', ',', '/', '|', '\\', '{', '}', '[', ']', '(', ')', '+', '-', '=', '%', '\'', '\"', '~', '`', '*', '<', '>', '^', ';', ':', '_'};
  private bool _IsValidName(string? name){
    if(name == null){
      return false;
    }
    if(name.Length >= 2){
      foreach(char ch in name){
        if(_unacceptableChars.Contains(ch)){
          return false;
        }
      }
    }
    return true;
  }
  private void _UncorrectNameLog(){
    Console.WriteLine("Try another name. The name can not be empty, and can not contain these characters: ");
    for(var i = 0; i < _unacceptableChars.Length; i ++){
      if(i != _unacceptableChars.Length - 1){
        Console.Write(_unacceptableChars[i] + ", ");
      }
      else{
        Console.Write(_unacceptableChars[i] + ".");
      }
    }
  }
  private void DefinePlayers(){
    FirstPlayerLoop:
    Console.WriteLine("Enter the first player's name: ");
    string? firstName = Console.ReadLine();
    if(_IsValidName(firstName)){
      SecondPlayerLoop:
      Console.WriteLine("Enter the second player's name: ");
      string? secondName = Console.ReadLine();
      if(_IsValidName(secondName)){
        if(_firstPlayer == null){
          _firstPlayer = new Player(firstName!);
          _secondPlayer = new Player(secondName!);
        }
        else{
          _firstPlayer.ChangeName(firstName!);
          _secondPlayer!.ChangeName(secondName!);
        }
      }
      else{
        _UncorrectNameLog();
        goto SecondPlayerLoop;
      }
    }
    else{
      _UncorrectNameLog();
      goto FirstPlayerLoop;
    }
  }
  private (byte posY, byte posX) _ConvertToCoords(byte position){
    switch(position){
      case 1:
        return (0, 0);
      case 2:
        return (0, 2);
      case 3:
        return (0, 4);
      case 4:
        return (2, 0);
      case 5:
        return (2, 2);
      case 6:
        return (2, 4);
      case 7:
        return (4, 0);
      case 8:
        return (4, 2);
      default:
        return (4, 4);
    }
  }
  private void _Move(){
    MoveLoop:
    Console.WriteLine("Choose where you want to move, from 1 to 9: ");
    if(byte.TryParse(Console.ReadLine(), out byte position)){
      if(position > 0 && position < 10){
        (byte posY, byte posX) = _ConvertToCoords(position);
        if(_IsEmpty(posY, posX)){
          _gameField[posY, posX] = symbol;
          Console.Clear();
          _InitializeGameField();
          if(_fiveTimesMove > 5){
            _IsGameOver();
          }
          else{
            _fiveTimesMove ++;
          }
          symbol = symbol == 'X' ? 'O' : 'X';
          return;
        }
      }
    }
    Console.WriteLine("Incorrect data. Wrong position number or the position is already taken.");
    goto MoveLoop;
  }
  public void ShowMenu(){
    DefinePlayers();
    MenuLoop:
    Console.WriteLine("Type anything to start a game.\nType \"change color\" to change menu color.\nType \"rename\" to rename characters.");
    string? inputs = Console.ReadLine();
    switch(inputs){
      case "change color":
        _ChangeColor();
      break;
      case "rename":
        DefinePlayers();
      break;
      default:
        Start();
      break;
    }
    goto MenuLoop;
  }
  private ConsoleColor[] _availableColors = new ConsoleColor[] { 
    ConsoleColor.Blue, ConsoleColor.DarkBlue, ConsoleColor.DarkGreen, ConsoleColor.DarkCyan, ConsoleColor.DarkRed, ConsoleColor.DarkMagenta, ConsoleColor.DarkYellow, ConsoleColor.Gray, ConsoleColor.DarkGray, ConsoleColor.Green, ConsoleColor.Cyan, ConsoleColor.Red, ConsoleColor.Magenta, ConsoleColor.Yellow, ConsoleColor.White
  };
  private void _ChangeColor(){
    Console.WriteLine("Available colors: ");
    for(var i = 0; i < _availableColors.Length; i ++){
      Console.ForegroundColor = _availableColors[i];
      Console.WriteLine(i + ". " + _availableColors[i]);
      Console.ResetColor();
    }
    Console.WriteLine("Type its number.");
    if(byte.TryParse(Console.ReadLine(), out byte inputs) && inputs < _availableColors.Length){
      Console.ForegroundColor = _availableColors[inputs];
    }
  }
  private void _ShowScores(){
    Console.WriteLine("Winner is " + (_isFirstPlayer == true ? _secondPlayer!.Name : _firstPlayer!.Name));
    Console.WriteLine(_firstPlayer!.Name + " " + _firstPlayer!.Score + " | " + _secondPlayer!.Name + " " + _secondPlayer!.Score);
  }
  private void Start(){
    _InitializeGameField();
    while(!_isGameOver){
      _Move();
    }
    _isGameOver = false;
    _ClearGameField();
    _ShowScores();
  }
}