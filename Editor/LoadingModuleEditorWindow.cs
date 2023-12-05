using System.Collections.Generic;
using System.Linq;
using LoadingModule.Contracts;
using LoadingModule.Editor.Entity.Utils;
using LoadingModule.Editor.NodeEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LoadingModule.Editor
{
    public class LoadingModuleEditorWindow : EditorWindow
    {
        private List<AbstractLoadingStepFactory> stepFactories;
        private Dictionary<string, AbstractLoadingStepFactory> factories;
        private NodeBasedEditor graphEditor;
        private IMGUIContainer imguiGraphContainer;
        private Label graphLabel;
        private VisualElement root;
        private AbstractLoadingStepFactory activeFactory;
    
        [MenuItem("Tools/Loading Module")]
        public static void ShowWindow()
        {
            LoadingModuleEditorWindow wnd = GetWindow<LoadingModuleEditorWindow>();
            wnd.titleContent = new GUIContent("Loading Module");
        }

        private void OnEnable()
        {
            InitVisualTreeAndApplyStyleSheet();

            graphEditor ??= CreateInstance<NodeBasedEditor>();
            factories = GetFactoryInstances();
            GenerateFactoriesButtons();
            graphLabel.text = factories.Keys.First();
            imguiGraphContainer.onGUIHandler += graphEditor.OnGUI;
        }

        private void OnDisable()
        {
            DestroyImmediate(graphEditor);
        }

        private void OnGUI()
        {
            root.Q<VisualElement>("Container").style.height = new 
                StyleLength(position.height);
            imguiGraphContainer.style.height = new StyleLength(position.height);
            imguiGraphContainer.onGUIHandler();
        }
        
        private void GenerateFactoriesButtons()
        {
            var factoryListView = root.Q<ListView>("FactoryList");
            factoryListView.makeItem = () => new FactoryButton();
            factoryListView.bindItem = (element, i) =>
            {
                var button = (FactoryButton) element;
                var factoryName = factories.Keys.ToList()[i];
                button.BindData(factoryName);
                button.clicked += () =>
                {
                    graphLabel.text = factoryName;
                    activeFactory = factories[factoryName];
                    graphEditor.SetFactory(activeFactory);
                };
            };
            factoryListView.itemsSource = factories.Keys.ToList();
            factoryListView.Rebuild();
        }
        private void InitVisualTreeAndApplyStyleSheet()
        {
            root = rootVisualElement;
            var visualTree = Resources.Load<VisualTreeAsset>("LoadingModuleMarkup");
            var styleSheet = Resources.Load<StyleSheet>("LoadingModuleStyle");
            visualTree.CloneTree(root);
            root.styleSheets.Add(styleSheet);
            graphLabel = root.Q<Label>("GraphLabel");
            imguiGraphContainer = root.Q<IMGUIContainer>("GraphContainer");
        }
        private static Dictionary<string, AbstractLoadingStepFactory> GetFactoryInstances()
        {
            var factories = FindUtils.GetVisibleFactoryInstances();
            return factories.ToDictionary(f => f.GetType().Name);
        }
        
    }
}