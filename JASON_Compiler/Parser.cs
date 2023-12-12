using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();
        
        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public  Node root;
        
        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        // Program -> Function_Statement Program | Main_Function
        Node Program()
        {
            Node program = new Node("Program");
            if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer+1].token_type == Token_Class.Main)
            {
                program.Children.Add(Main_Function());
            }
            else
            {
                program.Children.Add(Function_Statement());
                program.Children.Add(Program());
            }

            return program;
        }
        // Main_Function-> Datatype main () Function_Body
        Node Main_Function()
        {
            Node mainFunction = new Node("MainFunction");
            // write your code here to check the header sructure
            mainFunction.Children.Add(Datatype());
            mainFunction.Children.Add(match(Token_Class.Main));
            mainFunction.Children.Add(match(Token_Class.Leftbracket));
            mainFunction.Children.Add(match(Token_Class.Rightbracket));
            mainFunction.Children.Add(Function_Body());

            return mainFunction;
        }
        Node Datatype() {
            return null;
        }

        Node Function_Body()
        {
            return null;
        }
        //Function_Statement-> Function_Declaration Function_Body
        Node Function_Statement()
        {
            Node function_statement = new Node("Function_Statement");
            function_statement.Children.Add(Function_Declaration());
            function_statement.Children.Add(Function_Body());

            return function_statement;
        }
        /*
         Function_Declaration -> Datatype FunctionName ( Param )
            Param -> Parameter Parameters 
            Parameters -> , Parameter Parameters | ε
         */
        Node Function_Declaration()
        {
            Node function_Declaration = new Node("Function_Declaration");
            function_Declaration.Children.Add(Datatype());
            function_Declaration.Children.Add(Function_Name());
            function_Declaration.Children.Add(match(Token_Class.Leftbracket));
            function_Declaration.Children.Add(Param());
            function_Declaration.Children.Add(match(Token_Class.Rightbracket));

            return function_Declaration;
        }
        Node Param()
        {
            Node param = new Node("Param");
            param.Children.Add(Parameter());
            param.Children.Add(Parameters());
            return param;
        }
        //  Parameters -> , Parameter Parameters | ε
        Node Parameters()
        {
            Node parameters = new Node("parameters");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.comma)
            {
                parameters.Children.Add(match(Token_Class.comma));
                parameters.Children.Add(Parameter());
                parameters.Children.Add(Parameters());
            }
            return parameters;
        }
        Node Parameter()
        { return null; }
        Node Function_Name()
        {
            Node function_name = new Node("Function_Name");
            function_name.Children.Add(match(Token_Class.Identifier));
            return function_name;
        }
        // Implement your logic here

        // Return_Statement-> return Expression ;
        Node Return_Statement()
        {
            Node return_statement = new Node("Return_Statement");
            return_statement.Children.Add(match(Token_Class.Return));
            return_statement.Children.Add(Expression());
            return_statement.Children.Add(match(Token_Class.semicolon));

            return return_statement;
        }

        Node Expression()
        {
            return null;
        }
        //Read_Statement-> read identifier ;
        Node Read_Statement()
        {
            Node read_Statement = new Node("Read_Statement");
            read_Statement.Children.Add(match(Token_Class.read));
            read_Statement.Children.Add(match(Token_Class.Identifier));
            read_Statement.Children.Add(match(Token_Class.semicolon));

            return read_Statement;
        }

        /*
         Write_Statement-> write Expression; | endl;
         */
        Node Write_Statement()
        {
            Node write_statement = new Node("Write_Statement");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.write)
            {
                write_statement.Children.Add(match(Token_Class.write));
                write_statement.Children.Add(Expression());
            }
            else
            {
                write_statement.Children.Add(match(Token_Class.endl));
            }
            write_statement.Children.Add(match(Token_Class.semicolon));
            return write_statement;
        }

        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString()  + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
