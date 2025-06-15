import './App.css';
import RoutesGroup from './components/routes/RoutesGroup';
import 'bootstrap/dist/css/bootstrap.min.css';
import Modal from "react-modal";

Modal.setAppElement("#root");

function App() {
  return (
    <div className="App">
      <div className="wrapper">
        <RoutesGroup />
      </div>
    </div>
  );
}

export default App;
