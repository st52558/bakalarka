/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package teameditor;

/**
 *
 * @author st52558
 */
import Team.Stat;
import Team.TymZakladniInfo;
import java.awt.Toolkit;
import java.io.ByteArrayInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.sql.*;
import java.util.ArrayList;
import javafx.scene.image.Image;

public class SQLiteJDBC {
    public SQLiteJDBC() throws SQLException {
        Connection c = null;

        try {
            Class.forName("org.sqlite.JDBC");
            c = DriverManager.getConnection("jdbc:sqlite:test.db");
        } catch (Exception e) {
            System.err.println(e.getClass().getName() + ": " + e.getMessage());
            System.exit(0);
        }
        System.out.println("Opened database successfully");
        //statement.executeUpdate("drop table stat");
        //statement.executeUpdate("create table stat (id_stat INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, jmeno VARCHAR(40) NOT NULL)");
        //statement.executeUpdate("create table tym_basic (id_tym INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, nazev VARCHAR(40) NOT NULL, id_stat_fk INTEGER NOT NULL, FOREIGN KEY (id_stat_fk) REFERENCES stat(id_stat))");
        
    }

    ArrayList<Stat> getStatesWhereIsAnyTeam() throws ClassNotFoundException, SQLException{
        
        ArrayList<Stat> seznam = new ArrayList<Stat>();
        Class.forName("org.sqlite.JDBC");
        Connection c = DriverManager.getConnection("jdbc:sqlite:test.db");
        Statement s = c.createStatement();
        
        //udělat join na státy
        ResultSet rs = s.executeQuery("select tym_basic.id_stat_fk,stat.jmeno from tym_basic inner join stat on stat.id_stat=tym_basic.id_stat_fk group by id_stat_fk");
        while (rs.next()){
            seznam.add(new Stat(rs.getInt("id_stat_fk"),rs.getString("jmeno")));
        }
        System.out.println(seznam.size());
        return seznam;
    }
    
    ArrayList<TymZakladniInfo> getTymyPodleStatu(int id_stat) throws ClassNotFoundException, SQLException{
        ArrayList<TymZakladniInfo> seznam = new ArrayList<TymZakladniInfo>();
        Class.forName("org.sqlite.JDBC");
        Connection c = DriverManager.getConnection("jdbc:sqlite:test.db");
        Statement s = c.createStatement();
        ResultSet rs = s.executeQuery("select * from tym_basic where id_stat_fk=" + id_stat);
        while (rs.next()){
            seznam.add(new TymZakladniInfo(rs.getInt("id_tym"),rs.getString("nazev"),rs.getInt("id_stat_fk")));
        }
        return seznam;
    }
    
    ArrayList<TymZakladniInfo> getTymyPodleNazvu(String nazev) throws ClassNotFoundException, SQLException{
        ArrayList<TymZakladniInfo> seznam = new ArrayList<TymZakladniInfo>();
        Class.forName("org.sqlite.JDBC");
        Connection c = DriverManager.getConnection("jdbc:sqlite:test.db");
        Statement s = c.createStatement();
        ResultSet rs = s.executeQuery("select * from tym_basic where nazev like'%" + nazev + "%'");
        while (rs.next()){
            seznam.add(new TymZakladniInfo(rs.getInt("id_tym"),rs.getString("nazev"),rs.getInt("id_stat_fk")));
        }
        return seznam;
    }
    
    public Image getTeamLogo(int id_tym) throws ClassNotFoundException, SQLException, IOException{
        Image i = null;
        Class.forName("org.sqlite.JDBC");
        Connection c = DriverManager.getConnection("jdbc:sqlite:test.db");
        Statement s = c.createStatement();
        FileOutputStream fos = null;
        ResultSet rs = s.executeQuery("select logo from tym_basic where id_tym=" + id_tym);
        while (rs.next()){
             byte[] imgArr=rs.getBytes("logo");  
                i = new Image(new ByteArrayInputStream(imgArr));  
        }
        
        return i;
    }
    
    void removeTym(int id_tym) throws ClassNotFoundException, SQLException{
        Class.forName("org.sqlite.JDBC");
        Connection c = DriverManager.getConnection("jdbc:sqlite:test.db");
        Statement s = c.createStatement();
        s.execute("delete from tym_basic where id_tym="+id_tym);
    }
}
