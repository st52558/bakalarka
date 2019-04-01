/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package Team;

/**
 *
 * @author st52558
 */
public class TymZakladniInfo {
    int id_tym;
    String nazev;
    int narodnost;

    public TymZakladniInfo(int id_tym, String nazev, int narodnost) {
        this.id_tym = id_tym;
        this.nazev = nazev;
        this.narodnost = narodnost;
    }

    public int getId_tym() {
        return id_tym;
    }

    public String getNazev() {
        return nazev;
    }

    public int getNarodnost() {
        return narodnost;
    }
    
    
}
