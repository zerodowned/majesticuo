/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 *
 * @author James
 */
import java.util.Collections.*;
public class Player {
    private int serial;
    public int type;
    private int x;
    private int y;
    private int z;
    public int maxhp;
    public int maxmana;
    public int maxstam;
    public int curhp;
    public int curmana;
    public int curstam;
    public int gold;
    public int weight;
    public int maxweight;
    public String name;
    public int sex;
    public int intel;
    public int str;
    public int dex;
public int flags;

   public int hue;

    public Player(int ObjectSerial, int type, int myX, int myY, int myZ) {
        this.serial = ObjectSerial;
        this.type = type;
        this.x = myX;
        this.y = myY;
        this.z = myZ;
        //this.status = myStatus;
    }

    Player() {
        //throw new UnsupportedOperationException("Not yet implemented");
    }
     public void setmaxhp(int my) {
            this.maxhp = my;
    }
     public void setmaxmana(int my) {
            this.maxmana = my;
    }
     public void setmaxstam(int my) {
            this.maxstam = my;
    }
    public void setcurhp(int my) {
            this.curhp = my;
    }
     public void setcurmana(int my) {
            this.curmana = my;
    }
    public void setcurstam(int my) {
            this.curstam = my;
    }
    public void setgold(int my) {
            this.gold = my;
    }
     public void setweight(int my) {
            this.weight = my;
    }
    public void setmaxweight(int my) {
            this.maxweight = my;
    }
    public void setX(int myX) {
            this.x = myX;
    }
    public void setserial(int ObjectSerial) {
        this.serial = ObjectSerial;
    }
    public void setZ(int myZ) {
        this.z = myZ;
    }
     public void setY(int myY) {
        this.y = myY;
    }
     public void settype(int type) {
        this.type = type;
    }



    public int getX() {
        return this.x;
    }
    public int getY() {
        return this.y;
    }
    public int getZ() {
        return this.z;
    }
     public int getserial() {
        return this.serial;
    }
     public int gettype() {
        return this.type;
    }



}
