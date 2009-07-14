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
    private int type;
    private int x;
    private int y;
    private int z;
    private int color;

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
