/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 *
 * @author James
 */
import java.util.Collections.*;
public class UOObject{

    public int serial;
    public int type;
    public int x;
    public int y;
    public int z;
    public int color;

    public UOObject(int ObjectSerial, int type, int myX, int myY, int myZ, int color) {
        this.serial = ObjectSerial;
        this.type = type;
        this.x = myX;
        this.y = myY;
        this.z = myZ;
        this.color = color;
    }
}

