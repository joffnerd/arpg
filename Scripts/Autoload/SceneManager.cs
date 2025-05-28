using ARPG.Scripts.Menus;
using Godot;
using Godot.Collections;

namespace ARPG.Scripts.Autoload;

public partial class SceneManager : Node
{
   public static SceneManager Instance { get; private set; }

   public readonly int LEVEL_H = 144;
   public readonly int LEVEL_W = 240;
   public readonly string VERSION = "1.0";

   [Signal]
   public delegate void LoadStartEventHandler(LoadingScreen loadingScreen);

   [Signal]
   public delegate void SceneAddedEventHandler(Node2D loadedScene, LoadingScreen loadingScreen);

   [Signal]
   public delegate void LoadCompleteEventHandler(Node2D loadedScene);


   [Signal]
   public delegate void ContentFinishedLoadingEventHandler(Node content); // content ??

   [Signal]
   public delegate void ContentInvalidEventHandler(string contentPath);

   [Signal]
   public delegate void ContentFailedToLoadEventHandler(string contentPath);

   public Main Main;
   public Player.Player Player;

   private PackedScene loadingScreenScene = GD.Load<PackedScene>("res://Scenes/Menus/LoadingScreen.tscn");
   private LoadingScreen loadingScreen;
   private string transition;
   private Vector2 transitionDirection;
   private string contentPath;
   private Timer loadProgressTimer;
   private Node loadSceneInto;
   private Node sceneToUnload;
   private bool loadingInProgress = false;

   public override void _Ready()
   {
      Instance = this;

      //Connect(SignalName.ContentFinishedLoading, new Callable(this, "OnContentFinishedLoading"));
      //Connect(SignalName.ContentInvalid, new Callable(this, "OnContentInvalid"));
      //Connect(SignalName.ContentFailedToLoad, new Callable(this, "OnContentFailedToLoad"));

      ContentFinishedLoading += OnContentFinishedLoading;
      ContentInvalid += OnContentInvalid;
      ContentFailedToLoad += OnContentFailedToLoad;
   }

   public async void OnContentFinishedLoading(Node incomingNode)
   {
      //GD.Print("SM - OnContentFinishedLoading " + incomingNode.Name);

      var incomingScene = (Node2D)incomingNode;
      //GD.Print("SM - incomingScene " + incomingScene.Name);

      if (incomingNode.Name == "Main")
      {
         Main = incomingScene as Main; // Set Main
      }

      var outgoingScene = sceneToUnload;
      if (outgoingScene != null)
      {
         if (outgoingScene.HasMethod("GetData") && incomingScene.HasMethod("ReceiveData"))
         {
            var data = outgoingScene.Call("GetData");
            incomingScene.Call("ReceiveData", data);
         }
      }

      loadSceneInto.AddChild(incomingScene);

      EmitSignal(SignalName.SceneAdded, incomingScene, loadingScreen);

      if (transition == "zelda")
      {
         var pos = new Vector2(transitionDirection.X * LEVEL_W, transitionDirection.Y * LEVEL_H);
         incomingScene.Position = pos;

         var tweenIn = GetTree().CreateTween();
         tweenIn.TweenProperty(incomingScene, "position", Vector2.Zero, 1).SetTrans(Tween.TransitionType.Sine);

         var tweenOut = GetTree().CreateTween();
         var vectorOffScreen = Vector2.Zero;
         vectorOffScreen.X = transitionDirection.X * LEVEL_W;
         vectorOffScreen.Y = transitionDirection.X * LEVEL_H;
         tweenOut.TweenProperty(outgoingScene, "position", vectorOffScreen, 1).SetTrans(Tween.TransitionType.Sine);
         //tweenIn.Connect(Tween.SignalName.Finished, Callable.From(() => TweenInFinished(incomingScene)));
         //tweenIn.Finished += () => TweenInFinished(incomingScene);

         await ToSignal(tweenIn, "finished");
      }

      if (sceneToUnload != null)
      {
         if (sceneToUnload != GetTree().Root)
         {
            sceneToUnload.QueueFree();
         }
      }

      var checkScene = incomingNode.Name == "Main" ? incomingScene.GetNode<Node2D>("CurrentMap").GetChild(0) : incomingScene;

      if (checkScene.HasMethod("InitScene"))
      {
         checkScene.Call("InitScene");
      }

      if (loadingScreen != null)
      {
         loadingScreen.FinishTransition();

         await ToSignal(loadingScreen.AnimationPlayer, "animation_finished");
      }

      if (checkScene.HasMethod("StartScene"))
      {
         checkScene.Call("StartScene");
      }

      loadingInProgress = false;
      EmitSignal(SignalName.LoadComplete, incomingScene);
   }

   public void OnContentInvalid(string path)
   {
      GD.PrintErr("Cannot load resource: " + path);
   }

   public void OnContentFailedToLoad(string path)
   {
      GD.PrintErr("Failed to load resource: " + path);
   }

   public void AddLoadingScreen(string transitionType = "fade_to_black")
   {
      //GD.Print("SM - AddLoadingScreen");

      transition = transitionType;
      if (transitionType == "no_transition")
      {
         transition = "no_to_transition";
      }

      loadingScreen = loadingScreenScene.Instantiate() as LoadingScreen;

      var holder = GetTree().Root.GetNode("LSHolder");
      holder.AddChild(loadingScreen);
      loadingScreen.StartTransition(transition);
   }

   public void SwapScenes(string sceneToLoad, Node loadInto = null, Node sceneUnload = null, string transitionType = "fade_to_black")
   {
      //GD.Print("SM - SwapScenes " + sceneToLoad);

      if (loadingInProgress)
      {
         GD.PushWarning("SceneManager is already loading something");
         return;
      }

      loadingInProgress = true;
      loadInto ??= GetTree().Root;

      loadSceneInto = loadInto;
      sceneToUnload = sceneUnload;

      AddLoadingScreen(transitionType);
      LoadContent(sceneToLoad);
   }

   public void SwapScenes(string sceneToLoad, Node loadInto, Node sceneUnload, Vector2 moveDir)
   {
      //GD.Print("SM - SwapScenes 2");

      if (loadingInProgress)
      {
         GD.PushWarning("SceneManager is already loading something");
         return;
      }

      transition = "zelda";
      loadSceneInto = loadInto;
      sceneToUnload = sceneUnload;
      transitionDirection = moveDir;
      LoadContent(sceneToLoad);
   }

   public async void LoadContent(string content)
   {
      //GD.Print("SM - LoadContent");

      EmitSignal(SignalName.LoadStart, loadingScreen);

      if (transition != "zelda")
      {
         await ToSignal(loadingScreen, "TransitionInComplete");
         //loadingScreen.TransitionInComplete += () => LoadingScreenTransitionInComplete(content);
      }

      contentPath = content;
      var loader = ResourceLoader.LoadThreadedRequest(contentPath);
      if (!ResourceLoader.Exists(contentPath) || loader != Error.Ok)
      {
         EmitSignal(SignalName.ContentInvalid, contentPath);
         return;
      }

      loadProgressTimer = new Timer
      {
         WaitTime = 0.2
      };
      loadProgressTimer.Timeout += MonitorLoadStatus;

      GetTree().Root.AddChild(loadProgressTimer);
      loadProgressTimer.Start();
   }

   public void MonitorLoadStatus()
   {
      //GD.Print("SM - MonitorLoadStatus");

      var loadProgress = new Array();
      var loadStatus = ResourceLoader.LoadThreadedGetStatus(contentPath, loadProgress);

      switch (loadStatus)
      {
         case ResourceLoader.ThreadLoadStatus.InvalidResource:
            EmitSignal(SignalName.ContentInvalid, contentPath);
            loadProgressTimer.Stop();
            break;
         case ResourceLoader.ThreadLoadStatus.InProgress:
            if (loadingScreen != null)
            {
               var p = loadProgress[0].AsInt32();
               loadingScreen.UpdateBar(p * 100);
            }
            break;
         case ResourceLoader.ThreadLoadStatus.Failed:
            EmitSignal(SignalName.ContentFailedToLoad, contentPath);
            loadProgressTimer.Stop();
            break;
         case ResourceLoader.ThreadLoadStatus.Loaded:
            loadProgressTimer.Stop();
            loadProgressTimer.QueueFree();
            PackedScene node = ResourceLoader.LoadThreadedGet(contentPath) as PackedScene;
            EmitSignal(SignalName.ContentFinishedLoading, node.Instantiate());
            break;
      }
   }
}