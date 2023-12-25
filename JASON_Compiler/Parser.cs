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
        public Node root;

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
            program.Children.Add(Function_Statments());
            program.Children.Add(Main_Function());
            return program;
        }
        // Main_Function-> Datatype main () Function_Body
        Node Function_Statments()
        {
            Node fns = new Node("Functions");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type != Token_Class.Main)
            {
                fns.Children.Add(Function_Statement());
                fns.Children.Add(Function_Statments());
            }
            else
            {
                return null;
            }
            return fns;
        }
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
            Node function_body = new Node("Function_Body");
            function_body.Children.Add(match(Token_Class.Leftprant));
            function_body.Children.Add(Statements());
            function_body.Children.Add(Return_Statement());
            function_body.Children.Add(match(Token_Class.Rightprant));
            return function_body;
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


        //If_Statement-> if Condition_Statement then Statements If_Statement_Tail
        //If_Statement_Tail-> Else_If_Statement | Else_Statement | end
        Node If_Statement()
        {
            Node if_statement = new Node("If_Statement");
            if_statement.Children.Add(match(Token_Class.If));
            if_statement.Children.Add(Condition_Statement());
            if_statement.Children.Add(match(Token_Class.then));
            if_statement.Children.Add(Statements());
            if_statement.Children.Add(If_Statement_Tail());
            return if_statement;
        }

        /*Condition_Statement->Condition Optional_Condition*/
        Node Condition_Statement()
        {
            Node condition_statement = new Node("Condition_Statement");
            condition_statement.Children.Add(Condition());
            condition_statement.Children.Add(Optional_Condition());
            return condition_statement;
        }
        /*Condition->identifier Condition_Operator Term*/
        Node Condition()
        {
            Node condition = new Node("Condition");
            condition.Children.Add(match(Token_Class.Identifier));
            condition.Children.Add(Condition_Operator());
            condition.Children.Add(Term());
            return condition;
        }
        /*Condition_Operator-> <>|<|>|=*/
        Node Condition_Operator()
        {
            Node condition_operator = new Node("Condition_Operator");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.NotEqualOp)
            {
                condition_operator.Children.Add(match(Token_Class.NotEqualOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
            {
                condition_operator.Children.Add(match(Token_Class.GreaterThanOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
            {
                condition_operator.Children.Add(match(Token_Class.LessThanOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.EqualOp)
            {
                condition_operator.Children.Add(match(Token_Class.EqualOp));
            }
            return condition_operator;
        }

        /*Optional_Condition-> Boolean_Operator Condition_Statement|ε*/
        Node Optional_Condition()
        {
            Node optional_condition = new Node("Optional_Condition");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Or || TokenStream[InputPointer].token_type == Token_Class.And))
            {
                optional_condition.Children.Add(Boolean_Operator());
                optional_condition.Children.Add(Condition_Statement());
                return optional_condition;
            }
            else
            {
                return null;
            }

        }
        /*Boolean_Operator-> && | ||*/
        Node Boolean_Operator()
        {
            Node boolean_operator = new Node("Boolean_Operator");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Or)
            {
                boolean_operator.Children.Add(match(Token_Class.Or));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.And)
            {
                boolean_operator.Children.Add(match(Token_Class.And));
            }

            return boolean_operator;
        }

        /*Statements -> Statement Statements_helper*/
        Node Statements()
        {
            Node statements = new Node("Statements");
            statements.Children.Add(Statement());
            statements.Children.Add(Statements_helper());
            return statements;
        }
        //read x;
        // statments_helper -> until cond | ;
        /*Statements_helper -> ; Statement Statements_helper | ε*/
        Node Statements_helper()
        {
            Node statements_helper = new Node("Statements_helper");
            bool isuntil = false;
            if (InputPointer - 4 >= 0 && InputPointer - 4 < TokenStream.Count && TokenStream[InputPointer - 4].token_type == Token_Class.until)
                isuntil = true;
            if (InputPointer < TokenStream.Count && (isuntil || TokenStream[InputPointer].token_type == Token_Class.semicolon))
            {
                if (!isuntil)
                    statements_helper.Children.Add(match(Token_Class.semicolon));
                statements_helper.Children.Add(Statement());
                statements_helper.Children.Add(Statements_helper());
                return statements_helper;
            }
            else
            {
                return null;
            }


        }

        /*Statment -> Comment_Statement | 
       Function_Call     |
       Assignment_Statement |
       Declaration_Statement |
       Write_Statement	|
       Read_Statement |
       If_Statement |
       Repeat_Statement |*/

        // me7tagen n3ml list ll statements
        // leh
        Node Statement()
        {
            Node statment = new Node("Statement");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.AssignOp)
                {
                    statment.Children.Add(Assignment_Statement());
                }
                else if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.Leftbracket)
                {
                    statment.Children.Add(Function_call());
                }
            }
            else if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type
                == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.STRING || TokenStream[InputPointer].token_type == Token_Class.Float))
            {
                statment.Children.Add(Declaration_Statement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.write)
            {
                statment.Children.Add(Write_Statement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.read)
            {
                statment.Children.Add(Read_Statement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.If)
            {

                statment.Children.Add(If_Statement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.repeat)
            {
                statment.Children.Add(Repeat_Statement());
            }
            return statment;
        }

        /*Repeat_Statement-> repeat Statements until Condition_Statement*/
        Node Repeat_Statement()
        {
            Node repeat_statement = new Node("Repeat_Statement");
            repeat_statement.Children.Add(match(Token_Class.repeat));
            repeat_statement.Children.Add(Statements());
            repeat_statement.Children.Add(match(Token_Class.until));
            repeat_statement.Children.Add(Condition_Statement());
            return repeat_statement;
        }
        /*If_Statement_Tail-> Else_If_Statement | Else_Statement | end*/
        Node If_Statement_Tail()
        {
            Node if_Statement_Tail = new Node("If_Statement_Tail");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.elseif)
            {
                if_Statement_Tail.Children.Add(Else_If_Statement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Else)
            {
                if_Statement_Tail.Children.Add(Else_Statement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.end)
            {
                if_Statement_Tail.Children.Add(match(Token_Class.end));
            }
            return if_Statement_Tail;
        }
        /*Else_If_Statement-> elseif Condition_Statement then  Statements If_Statement_Tail*/
        Node Else_If_Statement()
        {
            Node else_if_statement = new Node("Else_If_Statement");
            else_if_statement.Children.Add(match(Token_Class.elseif));
            else_if_statement.Children.Add(Condition_Statement());
            else_if_statement.Children.Add(match(Token_Class.then));
            else_if_statement.Children.Add(Statements());
            else_if_statement.Children.Add(If_Statement_Tail());
            return else_if_statement;
        }
        /*Else_Statement-> else Statements end*/
        Node Else_Statement()
        {
            Node else_statement = new Node("Else_Statement");
            else_statement.Children.Add(match(Token_Class.Else));
            else_statement.Children.Add(Statements());
            else_statement.Children.Add(match(Token_Class.end));
            return else_statement;
        }
        Node Param()
        {
            Node param = new Node("Param");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.STRING))
            {
                param.Children.Add(Parameter());
                param.Children.Add(Parameters());

            }
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

        //Read_Statement-> read identifier ;
        Node Read_Statement()
        {
            Node read_Statement = new Node("Read_Statement");
            read_Statement.Children.Add(match(Token_Class.read));
            read_Statement.Children.Add(match(Token_Class.Identifier));
            //read_Statement.Children.Add(match(Token_Class.semicolon));

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
                if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.endl)
                {
                    write_statement.Children.Add(match(Token_Class.endl));
                }
                else
                {
                    write_statement.Children.Add(Expression());
                }
            }

            //write_statement.Children.Add(match(Token_Class.semicolon));
            return write_statement;
        }
        //Term->Number|identifier|Function_Call|(Equation)
        Node Term()
        {
            Node term = new Node("Term");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Leftbracket)
            {
                term.Children.Add(match(Token_Class.Leftbracket));
                term.Children.Add(Equation());
                term.Children.Add(match(Token_Class.Rightbracket));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Number)
            {
                term.Children.Add(match(Token_Class.Number));

            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {

                if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.Leftbracket)
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
        //ahhhhhhhhhhhhhh
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

                if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.AssignOp)
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
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Int)
            {
                datatype.Children.Add(match(Token_Class.Int));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Float)
            {
                datatype.Children.Add(match(Token_Class.Float));
            }
            else
            {
                datatype.Children.Add(match(Token_Class.STRING));
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
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.String)
            {
                expression.Children.Add(match(Token_Class.String));
            }
            else if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Number || TokenStream[InputPointer].token_type == Token_Class.Identifier))
            {
                if (InputPointer + 1 < TokenStream.Count &&
                    (TokenStream[InputPointer + 1].token_type == Token_Class.DivideOp ||
                    TokenStream[InputPointer + 1].token_type == Token_Class.MultiplyOp ||
                    TokenStream[InputPointer + 1].token_type == Token_Class.PlusOp ||
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
        /*
         Equation     → Term B
         B    → Arithmatic_Operator Term B | ε
         

         */
        Node Equation()
        {
            //Equation->Term B | ( Term B) | B X
            // Equation         → Term B
            Node equation = new Node("Equation");
            equation.Children.Add(Term());
            equation.Children.Add(B());

            return equation;
        }

        Node B()
        {

            //B->Arithmatic_Operator Term X
            //B                → Arithmatic_Operator Term B | ε
            Node b = new Node("B");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.PlusOp || TokenStream[InputPointer].token_type == Token_Class.MinusOp || TokenStream[InputPointer].token_type == Token_Class.MultiplyOp || TokenStream[InputPointer].token_type == Token_Class.DivideOp))
            {
                b.Children.Add(Arithmatic_Operator());
                b.Children.Add(Term());
                b.Children.Add(B());
                return b;

            }
            return null;
        }

        Node Arithmatic_Operator()
        {
            //Arithmatic_Operator -> + | - | * | / 
            Node arithmatic_operator = new Node("Arithmatic_Operator");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.PlusOp)
            {
                arithmatic_operator.Children.Add(match(Token_Class.PlusOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.MinusOp)
            {
                arithmatic_operator.Children.Add(match(Token_Class.MinusOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
            {
                arithmatic_operator.Children.Add(match(Token_Class.MultiplyOp));
            }
            else
            {
                arithmatic_operator.Children.Add(match(Token_Class.DivideOp));
            }
            return arithmatic_operator;
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
                        + ExpectedToken.ToString() + "\r\n");
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
