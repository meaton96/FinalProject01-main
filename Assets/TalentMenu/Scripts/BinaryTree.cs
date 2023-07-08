using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.UIElements;

internal class BinaryTree {

    private readonly Node<GameObject> root;

    /// <summary>
    /// create a new empty binary tree
    /// </summary>
    public BinaryTree() { }

    /// <summary>
    /// create a new binary tree from an array or a variable number of params
    /// </summary>
    /// <param name="values">the array of params to covert to binary tree</param>
    public BinaryTree(params GameObject[] values) {
        foreach (GameObject data in values) {
            root = Add(data, root);
        }
    }
    /// <summary>
    /// Create a new binary tree from any enumerable type
    /// </summary>
    /// <param name="values">the enumerable type to convert to a binary tree</param>
    public BinaryTree(IEnumerable<GameObject> values) {
        foreach (GameObject data in values) {
            root = Add(data, root);
        }
    }
    /// <summary>
    /// Adds a new node to the tree
    /// </summary>
    /// <param name="data">The data to add as a new node to the tree</param>
    /// <param name="root">The root node of the tree</param>
    /// <returns>The new node that is created with the data value</returns>
    public Node<GameObject> Add(GameObject data, Node<GameObject> root) {
        if (root == null) {
            return new Node<GameObject>(data);
        }
        if (data.GetComponent<Talent>() < root.Data.GetComponent<Talent>())
            root.Left = Add(data, root.Left).SetParent(root);
        else
            root.Right = Add(data, root.Right).SetParent(root);
        return root;
    }
    /// <summary>
    /// Prints the current tree in order
    /// </summary>
    /// <param name="root">the root node of the tree</param>
    public void InOrderPrint(Node<GameObject> root) {
        if (root == null)
            return;
        InOrderPrint(root.Left);
        Console.Write(root.Data + " ");
        InOrderPrint(root.Right);
    }

    /// <summary>
    /// recursively iterate through all nodes in the tree and sets their children's transforms
    /// </summary>
    /// <param name="root">root node of the binary tree</param>
    /// <param name="offsetX">X offset from the current node to draw the child at</param>
    /// <param name="offsetY">Y offset from the current node</param>
    /// <param name="bracketOffsetX">X offset to draw the bracket at</param>
    /// <param name="bracketOffsetY">Y offset to draw the bracket at</param>
    /// <param name="leftBracketPreFab">left bracket game object </param>
    /// <param name="rightBracketPreFab">right bracket game object</param>
    public void SetNodeTransforms(Node<GameObject> root, float offsetX, float offsetY,
        float bracketOffsetX, float bracketOffsetY, GameObject leftBracketPreFab, GameObject rightBracketPreFab, GameObject parent) {
        if (root == null)
            return;
        Vector2 pos = root.Data.transform.position;
        if (root.Left != null) {                                        //check left child
            GameObject temp = UnityEngine.Object.Instantiate(leftBracketPreFab, new Vector2(
                pos.x - bracketOffsetX,
                pos.y + bracketOffsetY),
                Quaternion.identity);
            temp.transform.parent = parent.transform;
            root.Left.Data.transform.position = new Vector2(pos.x - offsetX, pos.y + offsetY);
            SetNodeTransforms(root.Left, offsetX, offsetY, bracketOffsetX, bracketOffsetY, leftBracketPreFab, rightBracketPreFab, parent);
        }
        if (root.Right != null) {                                       //check right child
            GameObject temp = UnityEngine.Object.Instantiate(rightBracketPreFab, new Vector2(
                pos.x + bracketOffsetX,
                pos.y + bracketOffsetY),
                Quaternion.identity);
            temp.transform.parent = parent.transform;                   
            root.Right.Data.transform.position = new Vector2(pos.x + offsetX, pos.y + offsetY);
            SetNodeTransforms(root.Right, offsetX, offsetY, bracketOffsetX, bracketOffsetY, leftBracketPreFab, rightBracketPreFab, parent);
        }

    }
    /// <summary>
    /// sets the status of all game objects in the nodes to active
    /// </summary>
    /// <param name="root">the root node of the binary search tree</param>
    public void ActivateAllNodes(Node<GameObject> root) {

        if (root == null) {
            return;
        }
        else if (root.Data.GetComponent<Talent>() == null) {
            return;
        }
        ActivateAllNodes(root.Left);
        root.Data.SetActive(true);
        ActivateAllNodes(root.Right);

    }

    /// <summary>
    /// finds and returns the node containing the given game object instance
    /// </summary>
    /// <param name="root">the root node of the binary tree</param>
    /// <param name="gameObject">the game object to search for</param>
    /// <returns>the node containing the game object instance</returns>
    public Node<GameObject> Find(Node<GameObject> root, GameObject gameObject) {

        if (root == null)
            return null;

        if (root.Data.Equals(gameObject)) {
            return root;
        }
        else if (root.Data.GetComponent<Talent>() < gameObject.GetComponent<Talent>())
            return Find(root.Left, gameObject);
        else
            return Find(root.Right, gameObject);
    }

    /// <summary>
    /// Deletes the node with the given data value
    /// </summary>
    /// <param name="root">root node of the tree</param>
    /// <param name="data">the data value to dlete from the tree</param>
    /// <returns>the node that is deleted if its found, otherwise returns null</returns>
    /// <exception cref="ArgumentNullException">If the data value is null throw exception</exception>
    public Node<GameObject> Delete(Node<GameObject> root, GameObject data) {
        if (data == null)
            throw new ArgumentNullException("data");
        if (root == null)
            return null;
        if (data.GetComponent<Talent>() < root.Data.GetComponent<Talent>())
            root.Left = Delete(root.Left, data);
        else if (data.GetComponent<Talent>() > root.Data.GetComponent<Talent>())
            root.Right = Delete(root.Right, data);
        else {
            if (root.Left == null)
                return root.Right;
            else if (root.Right == null)
                return root.Left;

            root.Data = InOrderSuccessor(root.Right);
            root.Right = Delete(root.Right, root.Data);
        }
        return root;

    }
    /// <summary>
    /// Gets the data from the next smallest node of the passed in node data value
    /// </summary>
    /// <param name="root">the root node to be compare data too</param>
    /// <returns>the data value of the smallest node (farthest left child of root node)</returns>
    private GameObject InOrderSuccessor(Node<GameObject> root) {
        GameObject small = root.Data;
        while (root.Left != null) {
            root = root.Left;
            small = root.Data;
        }
        return small;
    }
    /// <summary>
    /// changes the binary tree to a list
    /// </summary>
    /// <param name="root">the root node</param>
    /// <param name="list">the list to populate, by reference</param>
    public void ToList(Node<GameObject> root, List<GameObject> list) {
        if (root == null) {
            return;
        }
        ToList(root.Left, list);
        list.Add(root.Data);
        ToList(root.Right, list);
    }


    /// <summary>
    /// Search the tree for the given data and get the node with that data value
    /// </summary>
    /// <param name="root">the root node of the tree</param>
    /// <param name="data">the data value to find in the tree</param>
    /// <returns>the Node with the given data value or null if it does not exist</returns>
    /// <exception cref="ArgumentNullException">If the data value is null throw exception</exception>
    public Node<GameObject> Search(Node<GameObject> root, GameObject data) {
        if (data == null)
            throw new ArgumentNullException("data");
        if (root == null)
            return null;

        if (data.GetComponent<Talent>() < root.Data.GetComponent<Talent>())
            return Search(root.Left, data);
        else if (data.GetComponent<Talent>() > root.Data.GetComponent<Talent>())
            return Search(root.Right, data);
        else
            return root;
    }

    public Node<GameObject> Root { get { return root; } }

}
public class Node<T> {
    public T Data;
    public Node<T> Left, Right;
    public Node<T> Parent;
    public Node(T data, Node<T> left, Node<T> right) {
        Data = data;
        Left = left;
        Right = right;
    }
    public Node(T data) {
        Data = data;
    }
    public Node<T> SetParent(Node<T> n) {
        Parent = n;
        return this;
    }
}
