import { Routes, Route } from "react-router-dom"
import Home from "./pages/Home"
import PlantDetail from "./pages/PlantDetail"

function App() {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/plant/:plantId" element={<PlantDetail />} />
    </Routes>
  )
}

export default App