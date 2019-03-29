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
public class Stat {
    int id_stat;
    String nazev;

    public Stat(int id_stat, String nazev) {
        this.id_stat = id_stat;
        this.nazev = nazev;
    }

    public int getId_stat() {
        return id_stat;
    }

    public void setId_stat(int id_stat) {
        this.id_stat = id_stat;
    }

    public String getNazev() {
        return nazev;
    }

    public void setNazev(String nazev) {
        this.nazev = nazev;
    }
}

