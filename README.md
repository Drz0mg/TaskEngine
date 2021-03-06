# TaskEngine -- Task based BotBase

## Introduction
The actions of your toon when not fighting or resting are controlled with scripts.

PScript is not a normal sequential scripts language. It's more of a definition of a number of tasks that should be performed. A few times per seconds the engine evaluates the scripts to determine which task to complete. Depending on settings within the Task File, the task can change at (almost) any moment.

## Definitions
A definition is like a variable in a normal programming languange. It is always preceeded by a $. These variables can only be defined once, and cannot (currently) be redefined.

```
$one = 1;
$two = 2;
$three = $one + $two;
// $three is now equal to 1+2 (3)
```

A definition is constantly being evaluated.

A definition is visible inside the task where it is defined and in subtasks to the task where it is defined.

It is possible to access a definition of a subtask if the subtask is named. A task can be named by adding a "Name : " before the task definition.

```
hotties : Hotspots
{
      $Locations = [[1,2,3]];
}
$locations_outside = $hotties.Locations;
```

## Built-In Definitions
```
$MyLevel // The level that your char currently is. Example: 31 (Ex. $cond = $MyLevel > 31;)
$MyRace // The race of your char. Example: Tauren (Ex. $cond = $MyRace == "Tauren";)
$MyClass // The class of your char. Example: Druid (Ex. $cond = $MyClass == "Druid";)

$MyTarget{"name"} // Returns the name of your current target
$MyTarget{"health"} // Returns the current health of your current target
$MyTarget{"mana"} // Returns the current mana of your current target
//etc... All things that can be called about your target from a custom class can be called from PScript in this form.

$IsInCombat // Returns if you are in combat. 1 for yes, 0 for no

$MyZone // The current zone your char is in. Example: Ashenvale
$MySubZone // The current sub zone your char is in. Example: Splintertree Post

$MyHealth // The current health of your char in percent. Example: 53;
$MyMana // The current amount of mana your char has in percent. Example: 12
$MyEnergy // Current amount of energy your char has in percent. Example: 78

$MyDurability // The durability of the armor you are wearing. 0 would be all red, 1 would be perfect condition. (Ex. $cond = $MyDurability < 0.3;)
$MyGearType // Returns the type of gear your char wears. Returns leather for hunter, rogue, and druid. Returns mail for warrior, paladin, and shaman. Returns cloth for everything else.

$MyX // Current X-Coord of your Char
$MyY // Current Y-Coord of your Char
$MyZ // Current Z-Coord of your Char
$FreeBagSlots // The number of empty slots in your bags
$IsStealthed // If char is stealthed. 1 for yes, 0 for no
$BGQueued // If you are currently queued. 1 for yes, 0 for no
$AlreadyTrained // If you have already trained at this level. 1 for yes, 0 for no

$ItemCount{"Item"} // Returns how many of "Item" you have in your bags
```

## Built-In Functions
```
QuestStatus("Quest Name") -- Returns the status of a quest. (accepted, failed, goaldone, completed)
BGQueued("BG Name") -- Returns 1 if queued for that BG, otherwise 0.
NearTo([X, Y, Z], 5.0) -- Checks if you are within 5 yards of X, Y, Z. Both parameters required (NearTo(Location loc, float close))
HaveBuff("BuffName") -- Checks if you have a buff by the name specified. HaveBuff("Arcane Intellect") == 0;
```

## Types
PScript has a very liberal type system (like perl).

Scalars are represented as strings or float.

Vectors are defined with the syntax [1,2,3].

Associative arrays (hashes) are accessed with the syntax: $id{key}, there is no way to define those in the language (yet). They are used by some internal definitions.

## Expressions
The expressions in PScript are very similar to those used in C.
```
+ - * / % ^ ++ -- < <= == >= > != && || 
( expr )
$id
$id{key}
function(parm1, param2, ...)
```

## Tasks
Tasks are organized in a tree like structure with parent tasks and child tasks. Some tasks have children (such as Seq and Par) while others do not (such as Hotspot or Vendor). How the children are treated depends totally on the parent task.

Tasks read definitions as parameters. What definitions depend on what task it is. The definition can be defined outside the task but will still be used as a parameter. For example $MinLevel can be defined at top level and will then be used by all Pull tasks in the script.

## Adding a New Task
To add a new task, you simply need to extend TaskEngine.Tasks.ParserTask (or one of its abstract sub-classes) and implement the desired behavior. The convention is to name your class XxxTask. Depending on the task's complexity you may also need to create an associated TaskEngine.Activities.Activity.

The task will automatically be available to psc files. It will be named Xxx, assuming you named the class XxxTask. More precisely, it will be named whatever your class is named with the word "Task" removed from the end, if it is there.

If you want the available name in psc files to be different from the class name or you want to make aliases for the task (e.g. ParTask can be Par or Parallel), define the following field in your class: 
public const string ParserKeyword = "...";
where the value is a comma-separated list of the aliases you would like available for the task.

## Adding a New Parser Function/Predefined Variable

### New Function

To add a new function (like QuestStatus() and NearTo()), create a new method in TaskEngine.Parser.Fcalls. Its signature must be 
public static Value FuncName(params Value[] args);
"FuncName" will be available to call from a psc file.

### New Predefined Variable

To add a new predefined variable (like $MyLevel and $FreeBagSlots), create a new method in TaskEngine.Parser.PredefinedVars. Its signature must be 
public static Value VarName();
"$VarName" will now be defined in psc files.

# Tasks

## DefendTask

### Defend

- Parameters: none
- Children: none
- Description: Fights back monsters that attack you, without this task in a script the toon will not fight back monsters that attack.

## HotspotTask

### Hotspots

- Parameters: $Locations, $Order, $UseMount, $HowClose
- Children: 1
- Description: Move the toon to the locations defined in $Locations. $Locations must be a array of arrays with 3 elements in each child array. Ex: [[1,2,3], [4,5,6]] The locations refer to in game coordinates ([X,Y,Z]). $Order can control the order in which the hostpots are visited. Setting it to "Order" will make it go to the locations in order. Setting to "Reverse" will make it go to the locations in reverse. If not specified it will be random. You can specify how close to the marked location the bot will run.

- If $UseMount is set to true it will mount up (provided the next location is over 50 yards away)
- If $HowClose is not set, the default value (3.5) will be used.

Example:
```
Hotspots 
{
    $Order = "Random"; // The order at which to go through the hotspots
    $Locations = [[ -4731, 1234, 105], [ -4733, 1461, 96]]]; // Set your hotspot locations here
    $UseMount = true; // Use a mount to move to the next hotspot if possible
    $HowClose = 2.5; // Get within 2.5 yards of the next hotspot.
}
```

## IfTask

### If

- Parameters: $cond
- Children: 1
- Description: This is a task that will activate its children only if $cond evaluates to true. If is finished when its child is finished or the $cond evaulates to false

Example:
```
If
{
  $cond = $MyClass == "Mage";
  // If you're a mage, do this...
}
```

## ParTask

### Par

- Parameters: $cond (optional)
- Children: many
- Description: Prioritized parallel consideration of what child to execute. This task can consider many task at once. It will read the definition $Prio of each child, the lower the value the more important the task is. If there are many task at the same priority it will choose the task that has its goal location closest. 

Example:
```
Par
{
  Defend { $Prio = 0; }
  Loot { $Prio = 1; }
  Danger { $Prio = 2; }
  // Defend has a higher priority so it will always try to defend itself, but if it would be logical to Loot before carrying on with Defend, it will do so. It will only pull if there's nothing already attacking or if you're looting.
}
```

## WhenTask

### When

- Parameters: $cond
- Children: 1
- Description: This is a task that will activate its children only if $cond evaluates to true. If is finished when its children are finished.

Example:
```
When
{
	$cond = ($MyLevel == 30);
	Log { $Text = "I am now level 30"; }
}
```
