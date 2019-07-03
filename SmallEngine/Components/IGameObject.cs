using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine.Input;
using SmallEngine.Messages;
using SmallEngine.Graphics;
using SmallEngine.Components;
using System.Runtime.Serialization;

namespace SmallEngine
{
    public interface IGameObject : IMessageReceiver, IDisposable
    {
        //TODO? http://archive.gamedev.net/archive/reference/programming/features/scenegraph/page2.html
        #region Properties
        /// <summary>
        /// Unique name given to a game object
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Position of the game object in world space
        /// </summary>
        Vector2 Position { get; set; }

        /// <summary>
        /// Scale of the game object
        /// </summary>
        Size Scale { get; set; }

        /// <summary>
        /// Rotation of the game object
        /// </summary>
        float Rotation { get; set; }

        /// <summary>
        /// Matrix representing the rotation in 
        /// </summary>
        Mathematics.Matrix2X2 RotationMatrix { get; }

        /// <summary>
        /// Matrix containing any other arbitrary transforms to perform on the object
        /// </summary>
        Mathematics.Matrix3X2 TransformMatrix { get; set; }

        /// <summary>
        /// Tag to put additional information about the game object
        /// </summary>
        string Tag { get; set; }

        /// <summary>
        /// The scene the game object was created in
        /// </summary>
        Scene ContainingScene { get; set; }

        /// <summary>
        /// Indicates the game object is destroyed and about to be removed from the scene
        /// </summary>
        bool Destroyed { get; }

        /// <summary>
        /// Index of the game object within the scene
        /// Used in part with Version to create a generational index
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// Version of the index within the scene
        /// Used in part with Index to create a generational index
        /// </summary>
        ushort Version { get; set; }
        #endregion

        //TODO sleep/awake?

        /// <summary>
        /// Retrieves an attached component with the specified type, if it exists
        /// </summary>
        IComponent GetComponent(Type pType);

        /// <summary>
        /// Retrieves all components attached to the game object
        /// </summary>
        IEnumerable<IComponent> GetComponents();

        /// <summary>
        /// Returns true if the game object has an attached component of type T
        /// </summary>
        bool HasComponent<T>() where T : class, IComponent;


        /// <summary>
        /// Adds a component to the game object
        /// </summary>
        void AddComponent(IComponent pComponent);

        /// <summary>
        /// Removes a component from the game object
        /// </summary>
        /// <param name="pComponent"></param>
        void RemoveComponent(Type pComponent);


        /// <summary>
        /// Provide any additional initialization required by the game object
        /// This method is called even when deserializing game objects
        /// so it's an appropriate spot to initialize variables
        /// </summary>
        void Initialize();

        /// <summary>
        /// Marks the game object for destruction
        /// Game object will be destroyed at a later point determined by the scene
        /// </summary>
        void Destroy();


        /// <summary>
        /// Returns a pointer to the current game object which can be used to retrieve the game object from the scene
        /// Use GetPointer instead of holding references to game objects since the later will prevent the object from being collected
        /// </summary>
        long GetPointer();
    }
}
