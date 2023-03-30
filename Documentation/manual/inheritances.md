# Иерархия наследований

Диаграмма наследований классов в игре
выглядит следующим образом

````mermaid
flowchart RL
O(Object)
P(Projectile)
B(BaseCharacter)
I(Item)
G(Gun)
EB(EnemyBase)
ES(EnemyStation)
C(Crystal)
P --> O
B --> O
I --> O
G --> O
EB --> O
ES --> O
C --> O

subgraph Projectiles
Bl(Bullet)
Gr(Grenade)
Ar(Arrow)
Bl --> P
Gr --> P
Ar --> P
end

subgraph Characters
EnC(EnemyCharacter)
PC(PlayerCharacter)
EnC --> B
PC --> B
end

subgraph Items
Mn(Money)
Gm(Gem)
Mn --> I
Gm --> I
end

subgraph Guns
Cr(Crossbow)
GrL(GrenadeLauncher)
Fr(Firearm)
Cr --> G
GrL --> G
Fr --> G
end
````