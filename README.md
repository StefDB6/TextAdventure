# TestRaiders_TextAdventure

## Overzicht
Dit project is een **text-based adventure game** ontwikkeld in C#.  
De speler verkent kamers, verzamelt items zoals sleutels en zwaarden, en probeert te overleven door de juiste keuzes te maken.  
Het spel bestaat uit meerdere klassen die samenwerken volgens principes van **objectgeoriënteerd programmeren** en **interfaces**.

---

## Structuur van het project
TestRaiders_TextAdventure/
├─ TestRaiders_TextAdventure.sln # Solution-bestand
│
├─ TestRaiders_TextAdventure/ # Hoofdproject
│ ├─ Core/
│ │ ├─ Interfaces/ # Alle interfaces (contracten)
│ │ │ ├─ IGame.cs
│ │ │ ├─ IInventory.cs
│ │ │ ├─ IItem.cs
│ │ │ ├─ IMonster.cs
│ │ │ ├─ IRoom.cs
│ │ │ ├─ IRoomsManager.cs
│ │ │ └─ IServiceCollection.cs
│ │ ├─ Models/ # Implementaties van de interfaces
│ │ │ ├─ Game.cs
│ │ │ ├─ GameSetup.cs
│ │ │ ├─ Inventory.cs
│ │ │ ├─ Item.cs
│ │ │ ├─ Monster.cs
│ │ │ ├─ Room.cs
│ │ │ ├─ RoomsManager.cs
│ │ │ └─ ServiceCollection.cs
│ │ ├─ Services/ # Eventuele extra logica of helpers
│ │ ├─ Direction.cs # Enum voor richtingen (north, south, ...)
│ │ ├─ ItemType.cs # Enum voor itemtypes (Key, Sword, ...)
│ │ └─ Program.cs # Startpunt van het spel
│ └─ Properties/
│
└─ Tests/ # Testproject
├─ DirectionTests.cs
├─ GameSetupTests.cs
├─ GameTests.cs
├─ InventoryTests.cs
├─ ItemTests.cs
├─ MonsterTests.cs
├─ MSTestSettings.cs
├─ RoomsManagerTests.cs
└─ RoomTests.cs

---

## Hoe het spel gestart kan worden

1. **Open de solution** `TestRaiders_TextAdventure.sln` in Visual Studio.  
2. Zorg dat de **startup project** is ingesteld op `TestRaiders_TextAdventure`.  
3. Klik op ▶️ **TetsRaiders_TextAventure_ (F5)** om het spel te runnen.  
4. Het spel start in de console en toont beschikbare commando’s.

---

## Spelcommando’s

go [north|south|east|west] → beweeg naar een aangrenzende kamer

take [item] → neem een item op (zoals sleutel of zwaard)

use [item] → gebruik een item (bv. open deur met sleutel)

inventory → toon de inhoud van de rugzak

look → bekijk de beschrijving van de kamer

help → toon beschikbare commando’s

quit → sluit het spel af

---

## Testaanpak

De Tests/ map bevat alle unit-, integratie- en behavior-driven tests.

---

Unit Tests

Testen individuele klassen of methodes:

InventoryTests – toevoegen/verwijderen van items, HasItem(type)

RoomTests – deadly rooms, locked doors

ItemTests – naam, beschrijving en type

DirectionTests – richtingen correct gedefinieerd

GameSetupTests – correcte initialisatie van startwereld

---

Integratie Tests

Controleren samenwerking tussen meerdere klassen:

Speler kan sleutel vinden en deur openen.

Speler kan zwaard vinden en monster verslaan.

Navigatie tussen kamers verloopt correct.

---

Behavior-Driven Tests

Scenario’s gebaseerd op gebruikersgedrag (given, when, then -> principe):

Given de speler staat in de kamer met de sleutel,
When take key,
Then zit de sleutel in de inventory.

Given de deur is vergrendeld,
When use key,
Then gaat de deur open.

Given de speler heeft geen zwaard,
When hij betreedt een monsterkamer,
Then volgt een game-over.