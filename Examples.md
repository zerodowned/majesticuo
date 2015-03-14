
```
//Written by James Kidd
// Simple Animal Lore example for my UO Client
            uonet.displaywipe();
            uonet.display("Script Started!");
            Event Events = new Event(uonet);
            int AnimalType = 2200;
            while (true)
            {

                Events.UseSkill(Skill.AnimalLore);
                while (uonet.UOClient.TargCurs == 0) { Thread.Sleep(10); }
                uoobject fi = Events.Finditem(AnimalType);
                if (fi.type == AnimalType)
                {
                    Events.Target(fi.x, fi.y, fi.z, fi.serial, fi.type);
                    Thread.Sleep(2000);
                }
                Thread.Sleep(10);
            
            }
```