# POE-TD
 
## A simple tower defense games with items.

The aim of the game is to build towers, equip items on them and defeat the waves.

Towers can be upgraded to increase various stats and item slots.

Towers have item slots for equiping items

Items are gained at the start of each round (choose one item from the options). (WIP: enemies have a very low chance to drop items).

### Towers:

* standard turret  -  a normal turret that shoots projectiles
* missile turret   -  a long range turret that shoots missiles that track enemies. Upon impact it explodes and deals damage to surrounding enemies
* laser turret     -  a short range turret that aims laser at the enemy. The damage is instance and keeps damaging over time as long as the laser is active. It also slows enemies.
* ice tower        -  a short range tower that damages all enemies in the area (WIP: slows enemies)
* (WIP: artilery   -  a long range tower that throws slow moving projectile and deals AOE damage the projectile leaves burning ground that inflicts damage over time.)

### Items:

* Increased Fire Rate - Adds a fire rate multiplier (1.5x) *doesn't work on laser tower*
* Increased Damage    - Adds a damage modifier (1.5x)
* Double attack       - deals an additional attack *doesn't work with laser tower* (WIP: currently are not stackable)
* Increased Range     - Adds a range multiplier (1.5x)

## Important restrictions

1. Tower selling refunds 50% of the base cost (WIP: increase refund gold for upgraded towers)
2. Selling towers with items on them won't return the items it has equiped.
3. Once an item has been equiped on the tower, it cannot be removed or retrieved. So choose wisely.
4. The inventory has 9 slots. Picking up any more items wont add it to the inventory and the item will be lost.
