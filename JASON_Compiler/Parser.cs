using System;
using System.Collections;
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

        //Node Expression()
        //{
        //    return null;
        //}
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
        //Term->Number|identifier|Function_Call
        Node Term()
        {
            Node term = new Node("Term");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Number)
            {
                term.Children.Add(match(Token_Class.Number));

            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                
                if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer+1].token_type == Token_Class.Leftbracket)
                {
                    term.Children.Add(Function_call());

                }
                else
                {
                    term.Children.Add(match(Token_Class.Identifier));

                }

            }

            return term;
        }
        //Function_Call-> identifer(Idlist)
        //Idlist-> identifier Idlist'
        //Idlist'-> , identifier Idlist' | ε
        Node Function_call()
        {
            Node function_call = new Node("Function_call");
            function_call.Children.Add(match(Token_Class.Identifier));
            function_call.Children.Add(match(Token_Class.Leftbracket));
            function_call.Children.Add(Idlist());
            function_call.Children.Add(match(Token_Class.Rightbracket));

            return function_call;
        }
        Node Idlist()
        {
            Node idlist = new Node("Idlist");
            idlist.Children.Add(match(Token_Class.Identifier));
            idlist.Children.Add(Idlist_dash());
            return idlist;
        }
        //Idlist'-> , identifier Idlist' | ε
        Node Idlist_dash()
        {
            Node idlist_dash = new Node("Idlist_dash");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.comma)
            {
                idlist_dash.Children.Add(match(Token_Class.comma));
                idlist_dash.Children.Add(match(Token_Class.Identifier));
                idlist_dash.Children.Add(Idlist_dash());

            }
            else
            {
                return null;

            }
           
            return idlist_dash;
        }
        //Assignment_Statement->Identifier := Expression
        Node Assignment_Statement()
        {
            Node assignment_statement = new Node("Assignment_Statement");
            assignment_statement.Children.Add(match(Token_Class.Identifier));
            assignment_statement.Children.Add(match(Token_Class.AssignOp));
            assignment_statement.Children.Add(Expression());


            return assignment_statement;

        }
        //Declaration_Statement->Datatype AssignmentList;

        //AssignmentList->identifier Assignments|Assignment_Statement Assignments
        //Assignments->ε|, AssignmentList
        Node Declaration_Statement()
        {
            Node declaration_statement = new Node("Declaration_Statement");
            declaration_statement.Children.Add(Datatype());
            declaration_statement.Children.Add(Assignment_list());



            return declaration_statement;
        }
        //AssignmentList->identifier Assignments|Assignment_Statement Assignments
        Node Assignment_list()
        {
            Node assignment_list = new Node("Assignment_list");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
              
                if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer+1].token_type == Token_Class.AssignOp)
                {
                    assignment_list.Children.Add(Assignment_Statement());
                    assignment_list.Children.Add(Assignments());

                }
                else
                {
               
                    assignment_list.Children.Add(match(Token_Class.Identifier));
                    assignment_list.Children.Add(Assignments());

                }

            }
            
                
         return assignment_list;
        }
        //Assignments->ε|, AssignmentList
        Node Assignments()
        {
            Node assignments = new Node("Assignment");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.comma)
            {
                assignments.Children.Add(match(Token_Class.comma));
                assignments.Children.Add(Assignment_list()); 

            }
            else
            {
                return null;
            }


                return assignments;

        }
        Node Datatype()
        {
            //DataType-> int | string | float
            Node datatype = new Node("Datatype");
            if (TokenStream[InputPointer].token_type== Token_Class.Int)
            {
                datatype.Children.Add(match(Token_Class.Int));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Float)
            {
                datatype.Children.Add(match(Token_Class.Float));
            }
            else
            {
                datatype.Children.Add(match(Token_Class.String));
            }
            return datatype;
        }

       
             Node Parameter()
        {
            //Parameter-> DataType identifier
            Node parameter = new Node("Parameter");
            parameter.Children.Add(Datatype());
            parameter.Children.Add(match(Token_Class.Identifier));
            return parameter;
        }
 
                Node Expression()
        {
            //Expression-> string | Term | Equation
            //Term->number|identifier|Function_Call
            //Function_Call-> identifer(Idlist)
            //Equation->Term B | ( Term B) | B
            Node expression = new Node("Expression");
            if (TokenStream[InputPointer].token_type==Token_Class.String)
            {
                expression.Children.Add(match(Token_Class.String));
            }
            else if (TokenStream[InputPointer].token_type==Token_Class.Number|| TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                if (InputPointer < TokenStream.Count && 
                    (TokenStream[InputPointer + 1].token_type == Token_Class.DivideOp||
                    TokenStream[InputPointer + 1].token_type == Token_Class.MultiplyOp||
                    TokenStream[InputPointer + 1].token_type == Token_Class.PlusOp||
                    TokenStream[InputPointer + 1].token_type == Token_Class.MinusOp))
                {
                    expression.Children.Add(Equation());
                }
                else
                {
                    expression.Children.Add(Term());
                }
            }
            else
            {
                expression.Children.Add(Equation());
            }
            return expression;
        }
       
                Node Equation()
        {
            //Equation->Term B | ( Term B) | B
            Node equation = new Node("Equation");
            if (TokenStream[InputPointer].token_type == Token_Class.Number || TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                equation.Children.Add(Term());
                equation.Children.Add(B());
            }
            else if (TokenStream[InputPointer].token_type==Token_Class.Leftbracket)
            {
                equation.Children.Add(match(Token_Class.Leftbracket));
                equation.Children.Add(Term());
                equation.Children.Add(B());
                equation.Children.Add(match(Token_Class.Rightbracket));
            }
            else
            {
                equation.Children.Add(B());
            }
            return equation;
        }
 
            Node B()
        {
            //B->Arithmatic_Operator Term
            Node b = new Node("B");
            b.Children.Add(Arithmatic_Operator());
            b.Children.Add(Term());
            return b;
        }
      
                Node Arithmatic_Operator()
        {
            //Arithmatic_Operator -> + | - | * | / 
            Node arithmatic_operator = new Node("Arithmatic_Operator");
            if (TokenStream[InputPointer].token_type==Token_Class.PlusOp)
            {
                arithmatic_operator.Children.Add(match(Token_Class.PlusOp));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.MinusOp)
            {
                arithmatic_operator.Children.Add(match(Token_Class.MinusOp));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
            {
                arithmatic_operator.Children.Add(match(Token_Class.MultiplyOp));
            }
            else
            {
                arithmatic_operator.Children.Add(match(Token_Class.DivideOp));
            }
            return arithmatic_operator;
        }

                Node Write_Statement()
        {
            Node write_statement = new Node("Write_Statement");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.comma)
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
