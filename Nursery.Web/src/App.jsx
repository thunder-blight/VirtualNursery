import { useState, useEffect } from "react";
import "./App.css";

function App() {
  const [plants, setPlants] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetch("https://localhost:7288/api/plants")
    .then(res => {
      if (!res.ok) throw new Error("Failed to fetch plants")
        return res.json()
    })
    .then(data => {
      setPlants(data)
      setLoading(false)
    })
    .catch(err => {
      setError(err.message)
      setLoading(false)
    })
  }, [])
  if (loading) return <div className="status">Loading plants...</div>
  if (error) return <div className="status error">Error: {error}</div>

  return (
    <div className="app">
      <header>
        <h1>🌿 Virtual Nursery</h1>
        <p className="subtitle">{plants.length} plants in the catalog</p>
      </header>
      <div className="plant-grid">
        {plants.map((plant, index) => (
          <div key={index} className="plant-card">
            <h2>{plant.name}</h2>
            <div className="plant-details">
              <div className="detail">
                <span className="label">Type</span>
                <span className="value">{plant.type}</span>
              </div>
              <div className="detail">
                <span className="label">Life Cycle</span>
                <span className="value">{plant.lifeCycle}</span>
              </div>
              <div className="detail">
                <span className="label">Flowering</span>
                <span className="value">{plant.floweringStatus ? "Yes" : "No"}</span>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

export default App;