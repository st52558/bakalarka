/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package teameditor;

import EditTeamForm.EditTeamForm;
import Team.Stat;
import Team.TymZakladniInfo;
import java.io.File;
import java.net.URL;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.ResourceBundle;
import java.util.logging.Level;
import java.util.logging.Logger;
import javafx.event.ActionEvent;
import javafx.event.Event;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.Button;
import javafx.scene.control.ComboBox;
import javafx.scene.control.Label;
import javafx.scene.control.ListView;
import javafx.scene.control.MenuItem;
import javafx.scene.control.TextField;
import javafx.scene.input.KeyEvent;
import javafx.scene.input.MouseEvent;
import javafx.stage.FileChooser;
import javafx.stage.Stage;

/**
 *
 * @author Gunny
 */
public class TeamEditorFXMLController implements Initializable {

    private Label label;
    private Button add;
    @FXML
    private ComboBox<String> narodnostCB;
    @FXML
    private TextField nazevTymuTF;
    @FXML
    private ListView<String> seznamTymuLB;
    @FXML
    private Button editTymB;
    @FXML
    private Button smazTymB;
    @FXML
    private Button PridatNovyTymB;
    ArrayList<Stat> staty;
    ArrayList<TymZakladniInfo> tymy;
    SQLiteJDBC f;
    Stage stage = new Stage();

    private void handleButtonAction(ActionEvent event) {
        System.out.println("You clicked me!");
        label.setText("Hello World!");
    }

    @Override
    public void initialize(URL url, ResourceBundle rb) {
        try {
            f = new SQLiteJDBC();
            staty = f.getStatesWhereIsAnyTeam();
            System.out.println(staty.size());
            for (int i = 0; i < staty.size(); i++) {
                narodnostCB.getItems().add(staty.get(i).getNazev());
            }

        } catch (SQLException ex) {
            Logger.getLogger(TeamEditorFXMLController.class.getName()).log(Level.SEVERE, null, ex);
        } catch (ClassNotFoundException ex) {
            Logger.getLogger(TeamEditorFXMLController.class.getName()).log(Level.SEVERE, null, ex);
        }
    }

    private void nacistTym(ActionEvent event) {

    }

    @FXML
    private void editujTym(ActionEvent event) throws Exception {
        new EditTeamForm(stage);
        stage.setAlwaysOnTop(true);
        disableButtons();
    }

    @FXML
    private void smazatTym(ActionEvent event) {
    }

    @FXML
    private void pridatTym(ActionEvent event) {
    }

    @FXML
    private void zmenaNarodnosti(Event event) throws ClassNotFoundException, SQLException {
        seznamTymuLB.getItems().clear();
        for (int i = 0; i < staty.size(); i++) {
            if (narodnostCB.getValue().equals(staty.get(i).getNazev())) {
                System.out.println(staty.get(i).getId_stat());
                tymy = f.getTymyPodleStatu(staty.get(i).getId_stat());
                for (int j = 0; j < tymy.size(); j++) {
                    seznamTymuLB.getItems().add(tymy.get(j).getNazev());
                }

            }
        }
    }

    @FXML
    private void vyhledavaniZmena(KeyEvent event) throws ClassNotFoundException, SQLException {
        seznamTymuLB.getItems().clear();
        tymy = f.getTymyPodleNazvu(nazevTymuTF.getText());
        for (int j = 0; j < tymy.size(); j++) {
            seznamTymuLB.getItems().add(tymy.get(j).getNazev());    
        }
    }

    private void disableButtons(){
        PridatNovyTymB.setDisable(true);
        editTymB.setDisable(true);
        smazTymB.setDisable(true);
    }
    
    private void enableButtons() {
        PridatNovyTymB.setDisable(false);
    editTymB.setDisable(false);
    smazTymB.setDisable(false);
    }
    
    private void active(){
        if (!stage.isShowing()){
            enableButtons();
        }
    }

    @FXML
    private void move(MouseEvent event) {
        active();
    }
}
