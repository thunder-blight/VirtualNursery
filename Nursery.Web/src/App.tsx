import { useState, useEffect, use } from "react"
import "./App.css"

interface Plant {
    name: string
    type: string
    lifeCycle: string
    floweringStatus: boolean
}

function App() {
    const [plants, setPlants] = useState<Plant[]>([])
    const [loading, setLoading] = useState<boolean>(true)
    const [error, setError] = useState<string | null>(null)

    useEffect(() => {
        fetch("https://localhost:7288/api/plants")
            .then((res: Response) => {
                if (!res.ok) throw new Error("Failed to fetch plants")
                return res.json()
            })
            .then((data: Plant[]) => {
                setPlants(data)
                setLoading(false)
            })
            .catch((err: Error) => {
                setError(err.message)
                setLoading(false)
            })
    }, [])

    if (loading) return <div className="status">Loading plants...</div>
    if (error) return <div className="status">Error: {error}</div>

    return (
        <div className="App">
            <header>
                <h1>🌿 Virtual Nursery</h1>
                <p className="subtitle">{plants.length} plants in the catalog</p>
            </header>
            <div className="plant-grid">
                {plants.map((plant: Plant, index: number) => (
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
                                <span className="label">Flowering Status</span>
                                <span className="value">{plant.floweringStatus ? "Yes" : "No"}</span>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    )
}

export default App