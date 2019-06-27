/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package EditTeamForm;

import teameditor.*;
import Team.Stat;
import Team.TymZakladniInfo;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;
import java.net.URL;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.ResourceBundle;
import java.util.logging.Level;
import java.util.logging.Logger;
import javafx.embed.swing.SwingFXUtils;
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
import javafx.scene.image.Image;
import javafx.scene.image.ImageView;
import javafx.scene.input.KeyEvent;
import javafx.stage.FileChooser;

/**
 *
 * @author Gunny
 */
public class EditTeamFormFXMLController implements Initializable {

    //private Label label;
    //private Button add;
    //ArrayList<Stat> staty;
    //ArrayList<TymZakladniInfo> tymy;
    SQLiteJDBC f;
    @FXML
    private ImageView logoImageView;

    private void handleButtonAction(ActionEvent event) {
    }

    @Override
    public void initialize(URL url, ResourceBundle rb) {
        try {
            f = new SQLiteJDBC();
            Image image = SwingFXUtils.toFXImage((BufferedImage)f.getTeamLogo(2), null);
            logoImageView.setImage(image);
                               

        } catch (SQLException ex) {
            Logger.getLogger(TeamEditorFXMLController.class.getName()).log(Level.SEVERE, null, ex);
        } catch (ClassNotFoundException ex) {
            Logger.getLogger(TeamEditorFXMLController.class.getName()).log(Level.SEVERE, null, ex);
        } catch (IOException ex) {
            Logger.getLogger(EditTeamFormFXMLController.class.getName()).log(Level.SEVERE, null, ex);
        }
    }

    private void nacistTym(ActionEvent event) {

    }

}
