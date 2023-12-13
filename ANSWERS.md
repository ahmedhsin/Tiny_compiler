# TASKS

## TASK 6
Function_Call-> identifer(Idlist)
Idlist-> identifier Idlist'
Idlist'-> , identifier Idlist' | ε
## TASK 7

Term->Number|identifier|Function_Call

## TASK 9
Equation->Term B | ( Term B) | B
B->Arithmatic_Operator Term
## TASK 10

Expression-> string | Term | Equation

## TASK 11
Assignment_Statement->Identifier := Expression
## TASK 13
Declaration_Statement->Datatype  AssignmentList ;

AssignmentList->identifier Assignments|Assignment_Statement Assignments
Assignments->ε|, AssignmentList
## TASK 14
Write_Statement-> write Expression; | endl;

## TASK 15
Read_Statement-> read identifier ;
## TASK 16
Return_Statement-> return Expression ;
## TASK 18

Condition->identifier Condition_Operator Term
Condition_Operator-> <>|<|>|=
## TASK 20
Condition_Statement->Condition Optional_Condition
Optional_Condition-> Boolean_Operator Condition_Statement|ε
Boolean_Operator-> && | ||
## TASK 21
If_Statement-> if Condition_Statement then Statements If_Statement_Tail
If_Statement_Tail-> Else_If_Statement | Else_Statement | end

## TASK 22
Else_If_Statement-> elseif Condition_Statement then Statement Statements If_Statement_Tail
## TASK 23
Else_Statement-> else Statements end
## TASK 24
Repeat_Statement-> repeat Statements until Condition_Statement
## TASK 25

Function_Name -> identifier

## TASK 26
Parameter-> DataType Identifier
DataType-> int | string | float
## TASK 27

Function_Declaration -> Datatype FunctionName ( Param )
       Param -> Parameter Parameters 
       Parameters -> , Parameter Parameters | ε

## TASK 28
Function_Body-> {Statements Return_Statement}
Statements -> Statement Statements_helper
Statements_helper -> ; Statement Statements_helper | ε
Statment -> Comment_Statement | 
	    Function_Call     |
	    Assignment_Statement |
	    Declaration_Statement |
	    Write_Statement	|
	    Read_Statement |
	    If_Statement |
	    Repeat_Statement |
## TASK 29

Function_Statement-> Function_Declaration Function_Body
## TASK 30
Main_Function-> Datatype main () Function_Body

## TASK 31
Program -> Function_Statement Program | Main_Function
